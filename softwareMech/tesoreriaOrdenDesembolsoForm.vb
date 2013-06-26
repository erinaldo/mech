﻿Imports System.Data
Imports System.Data.SqlClient
Imports ComponentesSolucion2008
Public Class tesoreriaOrdenDesembolsoForm
    Dim BindingSource0 As New BindingSource
    Dim BindingSource1 As New BindingSource
    Dim BindingSource2 As New BindingSource
    Dim BindingSource3 As New BindingSource
    Dim BindingSource4 As New BindingSource

    Private Sub tesoreriaOrdenDesembolsoForm_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Leave
        Me.Close()
    End Sub

    Private Sub tesoreriaOrdenDesembolsoForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Realizando la conexion con SQL Server - ConexionModule.vb
        'conexion() 'active esta linea si desea ejecutar el form independientemente RAS
        'AsignarColoresFormControles()
        VerificaConexion()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        'instanciando los dataAdapter con sus comandos select - DatasetAlmacenModule.vb
        Dim sele As String = "select idOP,estApro,nom,fecDes,serie,nro,simbolo,monto,montoDet,razon,nombre,hist,estDesem,codPersDes,estado,codMon,datoReq from VOrdenDesemTesoreria" 'order by idOP
        crearDataAdapterTable(daTabla1, sele)
        'daTabla1.SelectCommand.Parameters.Add("@ser", SqlDbType.VarChar, 5).Value = vSerie

        sele = "select codPagD,fecPago,tipoP,pagoDet,simbolo,montoPago,codTipP,codMon,idOP,idCue from VPagoDesemTesoreria where idOP=@idOP"
        crearDataAdapterTable(daDetDoc, sele)
        daDetDoc.SelectCommand.Parameters.Add("@idOP", SqlDbType.Int, 0).Value = 0

        sele = "select codMon,moneda,simbolo from TMoneda"
        crearDataAdapterTable(daTMon, sele)

        sele = "select codTipP,tipoP from TTipoPago"
        crearDataAdapterTable(daTabla2, sele)

        sele = "select idCue,banmon,banco,nroCue,simbolo,moneda,codBan,codMon from VBancoCuenta order by banco,simbolo"
        crearDataAdapterTable(daTabla3, sele)

        Try
            'procedimiento para instanciar el dataSet - DatasetAlmacenModule.vb
            crearDSAlmacen()
            'llenat el dataSet con los dataAdapter
            daTabla1.Fill(dsAlmacen, "VOrdenDesemTesoreria")
            daDetDoc.Fill(dsAlmacen, "VPagoDesemTesoreria")
            daTMon.Fill(dsAlmacen, "TMoneda")
            daTabla2.Fill(dsAlmacen, "TTipoPago")
            daTabla3.Fill(dsAlmacen, "VBancoCuenta")

            BindingSource1.DataSource = dsAlmacen
            BindingSource1.DataMember = "VOrdenDesemTesoreria"
            Navigator1.BindingSource = BindingSource1
            dgTabla1.DataSource = BindingSource1
            BindingSource1.Sort = "idOP"

            BindingSource2.DataSource = dsAlmacen
            BindingSource2.DataMember = "VPagoDesemTesoreria"
            Navigator2.BindingSource = BindingSource2
            dgTabla2.DataSource = BindingSource2
            'dgTabla2.SelectionMode = DataGridViewSelectionMode.FullRowSelect 'Seleccionar fila completa
            ModificarColumnasDGV()

            BindingSource0.DataSource = dsAlmacen
            BindingSource0.DataMember = "TMoneda"
            cbMon.DataSource = BindingSource0
            cbMon.DisplayMember = "simbolo"
            cbMon.ValueMember = "codMon"

            BindingSource3.DataSource = dsAlmacen
            BindingSource3.DataMember = "TTipoPago"
            cbMod.DataSource = BindingSource3
            cbMod.DisplayMember = "tipoP"
            cbMod.ValueMember = "codTipP"

            BindingSource4.DataSource = dsAlmacen
            BindingSource4.DataMember = "VBancoCuenta"
            cbCue.DataSource = BindingSource4
            cbCue.DisplayMember = "banmon"
            cbCue.ValueMember = "idCue"

            configurarColorControl()

            vfVan1 = True
            visualizarDet()

            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
            Me.Close()
            Exit Sub
        End Try
    End Sub

    Private Sub tesoreriaOrdenDesembolsoForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        colorearFila()
    End Sub

    Private Sub cbCue_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbCue.SelectedIndexChanged
        cbMon.SelectedValue = BindingSource4.Item(cbCue.SelectedIndex)(7)
        txtDes.Text = cbCue.Text.Trim()
    End Sub

    Private Sub colorearFila()
        For j As Short = 0 To BindingSource1.Count - 1
            If BindingSource1.Item(j)(12) = 1 Then 'Aprobado
                dgTabla1.Rows(j).Cells(1).Style.BackColor = Color.Green 'Color.YellowGreen
                dgTabla1.Rows(j).Cells(1).Style.ForeColor = Color.White
            End If
            If BindingSource1.Item(j)(12) = 2 Then 'Observado
                dgTabla1.Rows(j).Cells(1).Style.BackColor = Color.Yellow
                dgTabla1.Rows(j).Cells(1).Style.ForeColor = Color.Red
            End If
            If BindingSource1.Item(j)(12) = 3 Then 'Rechazado
                dgTabla1.Rows(j).Cells(1).Style.BackColor = Color.Red
                dgTabla1.Rows(j).Cells(1).Style.ForeColor = Color.White
            End If
        Next
    End Sub

    Private Sub ModificarColumnasDGV()
        With dgTabla1
            .Columns(0).Visible = False
            .Columns(1).Width = 80
            .Columns(1).HeaderText = "Estado"
            .Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns(2).Width = 100
            .Columns(2).HeaderText = "Aprobo"
            .Columns(3).Width = 70
            .Columns(3).HeaderText = "Fecha"
            .Columns(4).HeaderText = "Serie"
            .Columns(4).Width = 40
            .Columns(5).HeaderText = "NºOrden"
            .Columns(5).Width = 50
            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns(6).HeaderText = ""
            .Columns(6).Width = 30
            .Columns(7).Width = 75
            .Columns(7).HeaderText = "Monto"
            .Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns(8).Width = 75
            .Columns(8).HeaderText = "Detracción"
            .Columns(8).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns(9).Width = 140
            .Columns(9).HeaderText = "Proveedor"
            .Columns(10).Width = 160
            .Columns(10).HeaderText = "Lugar / Obra"
            .Columns(11).Width = 400
            .Columns(11).HeaderText = ""
            .Columns(12).Visible = False
            .Columns(13).Visible = False
            .Columns(14).Visible = False
            .Columns(15).Visible = False
            .Columns(16).Visible = False
            .ColumnHeadersDefaultCellStyle.BackColor = HeaderBackColorP
            .ColumnHeadersDefaultCellStyle.ForeColor = HeaderForeColorP
            .RowHeadersDefaultCellStyle.BackColor = HeaderBackColorP
            .RowHeadersDefaultCellStyle.ForeColor = HeaderForeColorP
        End With
        With dgTabla2
            .Columns(0).Visible = False
            .Columns(1).Width = 75
            .Columns(1).HeaderText = "Fecha"
            .Columns(2).Width = 120
            .Columns(2).HeaderText = "Modalidad"
            .Columns(3).HeaderText = "Descripción Pago"
            .Columns(3).Width = 300
            .Columns(4).HeaderText = ""
            .Columns(4).Width = 30
            .Columns(5).Width = 75
            .Columns(5).HeaderText = "Monto"
            .Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns(6).Visible = False
            .Columns(7).Visible = False
            .Columns(8).Visible = False
            .Columns(9).Visible = False
            .ColumnHeadersDefaultCellStyle.BackColor = HeaderBackColorP
            .ColumnHeadersDefaultCellStyle.ForeColor = HeaderForeColorP
            .RowHeadersDefaultCellStyle.BackColor = HeaderBackColorP
            .RowHeadersDefaultCellStyle.ForeColor = HeaderForeColorP
        End With
    End Sub

    Private Sub configurarColorControl()
        Me.BackColor = BackColorP
        Me.lblTitulo.BackColor = TituloBackColorP
        Me.lblTitulo.ForeColor = HeaderForeColorP
        Me.lblDerecha.BackColor = TituloBackColorP
        Me.lblDerecha.ForeColor = HeaderForeColorP
        Me.Text = nomNegocio
        Label1.ForeColor = ForeColorLabel
        Label2.ForeColor = ForeColorLabel
        Label3.ForeColor = ForeColorLabel
        Label4.ForeColor = ForeColorLabel
        Label5.ForeColor = ForeColorLabel
        Label6.ForeColor = ForeColorLabel
        Label7.ForeColor = ForeColorLabel
        btnNuevo.ForeColor = ForeColorButtom
        btnModificar.ForeColor = ForeColorButtom
        btnCancelar.ForeColor = ForeColorButtom
        btnEliminar.ForeColor = ForeColorButtom
        btnCerrar.ForeColor = ForeColorButtom
    End Sub

    Private Sub dgTabla1_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgTabla1.CurrentCellChanged
        visualizarDet()
    End Sub

    Dim vfVan1 As Boolean = False
    Private Sub visualizarDet()
        If vfVan1 Then
            If BindingSource1.Position = -1 Then
                dsAlmacen.Tables("VPagoDesemTesoreria").Clear()
                Exit Sub
            End If
            Me.Cursor = Cursors.WaitCursor
            dsAlmacen.Tables("VPagoDesemTesoreria").Clear()
            daDetDoc.SelectCommand.Parameters("@idOP").Value = BindingSource1.Item(BindingSource1.Position)(0)
            daDetDoc.Fill(dsAlmacen, "VPagoDesemTesoreria")
            'colorear()
            sumTotal()
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub sumTotal()
        If BindingSource2.Position = -1 Then
            txtTotal.Text = "0.00"
            Exit Sub
        End If
        txtTotal.Text = dsAlmacen.Tables("VPagoDesemTesoreria").Compute("Sum(montoPago)", Nothing)
    End Sub

    Private Sub btnCerrar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCerrar.Click
        dgTabla1.Dispose()
        dgTabla2.Dispose()
        Me.Close()
    End Sub

    Private Sub desactivarControles1()
        Panel1.Enabled = False
        If vfNuevo2 = "guardar" Then
            btnModificar.Enabled = False
            btnModificar.FlatStyle = FlatStyle.Flat
        Else    'Se presiono <Modificar>
            btnNuevo.Enabled = False
            btnNuevo.FlatStyle = FlatStyle.Flat
        End If
        btnCancelar.Enabled = True
        btnCancelar.FlatStyle = FlatStyle.Standard
        btnEliminar.Enabled = False
        btnEliminar.FlatStyle = FlatStyle.Flat
        btnCerrar.Enabled = False
        btnCerrar.FlatStyle = FlatStyle.Flat
    End Sub

    Private Sub activarControles1()
        Panel1.Enabled = True
        btnNuevo.Enabled = True
        btnNuevo.FlatStyle = FlatStyle.Standard
        btnModificar.Enabled = True
        btnModificar.FlatStyle = FlatStyle.Standard
        btnCancelar.Enabled = False
        btnCancelar.FlatStyle = FlatStyle.Flat
        btnEliminar.Enabled = True
        btnEliminar.FlatStyle = FlatStyle.Standard
        btnCerrar.Enabled = True
        btnCerrar.FlatStyle = FlatStyle.Standard
    End Sub

    Private Sub activarText()
        txtDes.ReadOnly = False
        cbMod.Enabled = True
        cbCue.Enabled = True
        txtMon.ReadOnly = False
    End Sub

    Private Sub desActivarText()
        cbMod.Enabled = False
        cbCue.Enabled = False
        txtDes.ReadOnly = True
        txtMon.ReadOnly = True
    End Sub

    Private Function ValidarCampos() As Boolean
        'Todas las funciones estan creadas en el module ValidarCamposModule.vb
        If validaCampoVacioMinCaracNoNumer(txtDes.Text.Trim, 3) Then
            txtDes.errorProv()
            Return True
        End If
        If ValidaNroMayorOigualCero(txtMon.Text) Then
            txtMon.errorProv()
            Return True
        End If

        'Todo OK RAS
        Return False
    End Function

    Dim vfNuevo2 As String = "nuevo"
    Private Sub btnNuevo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNuevo.Click
        If dgTabla1.Rows.Count = 0 Then
            StatusBarClass.messageBarraEstado("  No existe Orden de Desembolso...")
            Exit Sub
        End If
        If vfNuevo2 = "nuevo" Then
            vfNuevo2 = "guardar"
            Me.btnNuevo.Text = "Guardar"
            desactivarControles1()
            activarText()
            txtDes.Clear()
            cbMod.Focus()
            StatusBarClass.messageBarraEstado("")
            Me.AcceptButton = Me.btnNuevo
        Else   ' guardar
            'validaCampoVacio... creado en el Module ValidarCamposModule.vb, 3=minimo de caractres
            If ValidarCampos() Then
                Exit Sub
            End If

            Dim finalMytrans As Boolean = False
            Dim wait As New waitForm
            wait.Show()
            Me.Cursor = Cursors.WaitCursor
            Dim myTrans As SqlTransaction = Cn.BeginTransaction()
            Try
                StatusBarClass.messageBarraEstado("  GUARDANDO DATOS...")
                Me.Refresh()

                'TPagoDesembolso
                comandoInsert()
                cmInserTable.Transaction = myTrans
                If cmInserTable.ExecuteNonQuery() < 1 Then
                    wait.Close()
                    Me.Cursor = Cursors.Default
                    myTrans.Rollback()
                    MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
                Dim codPagD As Integer = cmInserTable.Parameters("@Identity").Value

                'confirma la transaccion
                myTrans.Commit()    'con exito RAS

                StatusBarClass.messageBarraEstado("  LOS DATOS FUERON GUARDADOS CON EXITO...")
                finalMytrans = True
                vfVan1 = False

                Me.btnCancelar.PerformClick()

                vfVan1 = True
                visualizarDet()

                'Buscando por nombre de campo y luego pocisionarlo con el indice
                BindingSource2.Position = BindingSource2.Find("codPagD", codPagD)

                'Clase definida y con miembros shared en la biblioteca ComponentesRAS
                StatusBarClass.messageBarraEstado("  Registro fué guardado con exito...")
                wait.Close()
                Me.Cursor = Cursors.Default
                colorearFila()
            Catch f As Exception
                wait.Close()
                Me.Cursor = Cursors.Default
                If finalMytrans Then
                    MessageBox.Show("NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Information)
                    Me.Close()
                    Exit Sub
                Else
                    myTrans.Rollback()
                    MessageBox.Show("tipoM de exception: " & f.Message & Chr(13) & "NO SE GUARDO LA INFORMACION PROCESADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
            End Try
        End If
    End Sub

    Private Sub btnCancelar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelar.Click
        vfNuevo2 = "nuevo"
        Me.btnNuevo.Text = "Nuevo"
        vfModificar2 = "modificar"
        Me.btnModificar.Text = "Modificar"
        activarControles1()
        desActivarText()
        'Clase definida y con miembros shared en la biblioteca ComponentesRAS
        StatusBarClass.messageBarraEstado("  Proceso cancelado...")
        enlazarText()
    End Sub

    Dim cmInserTable As SqlCommand
    Private Sub comandoInsert()
        cmInserTable = New SqlCommand
        cmInserTable.CommandType = CommandType.StoredProcedure
        cmInserTable.CommandText = "PA_InsertTPagoDesembolso"
        cmInserTable.Connection = Cn
        cmInserTable.Parameters.Add("@fec", SqlDbType.Date).Value = date1.Value.Date
        cmInserTable.Parameters.Add("@codT", SqlDbType.Int, 0).Value = cbMod.SelectedValue
        cmInserTable.Parameters.Add("@pago", SqlDbType.VarChar, 100).Value = txtDes.Text
        cmInserTable.Parameters.Add("@codM", SqlDbType.Int, 0).Value = cbMon.SelectedValue
        cmInserTable.Parameters.Add("@monto", SqlDbType.Decimal, 0).Value = txtMon.Text
        cmInserTable.Parameters.Add("@idOP", SqlDbType.Int, 0).Value = BindingSource1.Item(BindingSource1.Position)(0)
        cmInserTable.Parameters.Add("@idC", SqlDbType.Int, 0).Value = cbCue.SelectedValue
        'configurando direction output = parametro de solo salida
        cmInserTable.Parameters.Add("@Identity", SqlDbType.Int, 0)
        cmInserTable.Parameters("@Identity").Direction = ParameterDirection.Output
    End Sub

    Dim cmInserTable2 As SqlCommand
    Private Sub comandoInsert2(ByVal idOP As Integer, ByVal codPers As Integer, ByVal estado As Integer, ByVal tipo As Integer, ByVal obs As String)
        cmInserTable2 = New SqlCommand
        cmInserTable2.CommandType = CommandType.Text
        cmInserTable2.CommandText = "insert into TPersDesem(idOP,codPers,estDesem,tipoA,obserDesem,fecFir) values(@id,@codP,@est,@tipo,@obs,@fec)"
        cmInserTable2.Connection = Cn
        cmInserTable2.Parameters.Add("@id", SqlDbType.Int, 0).Value = idOP
        cmInserTable2.Parameters.Add("@codP", SqlDbType.Int, 0).Value = codPers 'vPass
        cmInserTable2.Parameters.Add("@est", SqlDbType.Int, 0).Value = estado '1=Aprobado
        cmInserTable2.Parameters.Add("@tipo", SqlDbType.Int, 0).Value = tipo '1=Solicitante
        cmInserTable2.Parameters.Add("@obs", SqlDbType.VarChar, 100).Value = obs
        cmInserTable2.Parameters.Add("@fec", SqlDbType.Date).Value = Now.Date
    End Sub

    Private Sub ToolStripButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton5.Click
        If BindingSource2.Position = -1 Then
            StatusBarClass.messageBarraEstado("Proceso denegado, No existe registro de pagos...")
            Exit Sub
        End If

        Dim finalMytrans As Boolean = False
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Try
            StatusBarClass.messageBarraEstado("  PROCESANDO DATOS...")
            Me.Refresh()

            'TPersDesem
            comandoInsert2(BindingSource1.Item(BindingSource1.Position)(0), vPass, 1, 3, "")  '1=aprobado  3=tesoreria
            cmInserTable2.Transaction = myTrans
            If cmInserTable2.ExecuteNonQuery() < 1 Then
                wait.Close()
                Me.Cursor = Cursors.Default
                myTrans.Rollback()
                MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If

            'TOrdenDesembolso
            comandoUpdate23(1) '1=Terminado 2=cerrado
            cmUpdateTable23.Transaction = myTrans
            If cmUpdateTable23.ExecuteNonQuery() < 1 Then
                'deshace la transaccion
                wait.Close()
                Me.Cursor = Cursors.Default
                myTrans.Rollback()
                MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If

            'confirma la transaccion
            myTrans.Commit()    'con exito RAS

            StatusBarClass.messageBarraEstado("  LOS DATOS FUERON CERRADOS CON EXITO...")
            finalMytrans = True
            vfVan1 = False

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesemTesoreria").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesemTesoreria")

            vfVan1 = True
            visualizarDet()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué cerrado con exito...")
            wait.Close()
            Me.Cursor = Cursors.Default
            colorearFila()
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            If finalMytrans Then
                MessageBox.Show("NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Information)
                Me.Close()
                Exit Sub
            Else
                myTrans.Rollback()
                MessageBox.Show("tipoM de exception: " & f.Message & Chr(13) & "NO SE GUARDO LA INFORMACION PROCESADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If
        End Try
    End Sub

    Dim cmUpdateTable23 As SqlCommand
    Private Sub comandoUpdate23(ByVal estado As Short)
        cmUpdateTable23 = New SqlCommand
        cmUpdateTable23.CommandType = CommandType.Text
        cmUpdateTable23.CommandText = "update TOrdenDesembolso set estado=@est where idOP=@nro"
        cmUpdateTable23.Connection = Cn
        cmUpdateTable23.Parameters.Add("@est", SqlDbType.Int, 0).Value = estado
        cmUpdateTable23.Parameters.Add("@nro", SqlDbType.Int, 0).Value = BindingSource1.Item(BindingSource1.Position)(0)
    End Sub

    Private Sub btnEliminar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEliminar.Click
        If dgTabla2.Rows.Count = 0 Then
            StatusBarClass.messageBarraEstado("  No existe registro a eliminar...")
            Exit Sub
        End If

        Dim finalMytrans As Boolean = False
        'creando una instancia de transaccion 
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Dim wait As New waitForm
        wait.Show()
        Try
            StatusBarClass.messageBarraEstado("  ELIMINANDO REGISTROS...")

            'Tabla TPagoDesembolso
            comandoDelete1()
            cmDeleteTable1.Transaction = myTrans
            If cmDeleteTable1.ExecuteNonQuery() < 1 Then
                wait.Close()
                myTrans.Rollback()
                MessageBox.Show("ERROR..No se puede eliminar producto...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If

            Me.Refresh()

            'confirma la transaccion
            myTrans.Commit()
            StatusBarClass.messageBarraEstado("  REGISTRO FUE ELIMINADO CON EXITO...")
            finalMytrans = True
            vfVan1 = False


            vfVan1 = True
            visualizarDet()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué eliminado con exito...")
            wait.Close()
            colorearFila()
        Catch f As Exception
            wait.Close()
            If finalMytrans Then
                MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Information)
                Me.Close()
                Exit Sub
            Else
                'deshace la transaccion
                myTrans.Rollback()
                MessageBox.Show("tipoM de exception: " & f.Message & Chr(13) & "NO SE ELIMINO EL REGISTRO SELECCIONADO...", nomNegocio, Nothing, MessageBoxIcon.Information)
            End If
        End Try
    End Sub

    Dim cmDeleteTable1 As SqlCommand
    Private Sub comandoDelete1()
        cmDeleteTable1 = New SqlCommand
        cmDeleteTable1.CommandType = CommandType.Text
        cmDeleteTable1.CommandText = "delete from TPagoDesembolso where codPagD=@cod"
        cmDeleteTable1.Connection = Cn
        cmDeleteTable1.Parameters.Add("@cod", SqlDbType.Int, 0).Value = BindingSource2.Item(BindingSource2.Position)(0)
    End Sub

    Dim vfModificar2 As String = "modificar"
    Private Sub btnModificar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnModificar.Click
        If vfModificar2 = "modificar" Then
            If dgTabla2.Rows.Count = 0 Then
                StatusBarClass.messageBarraEstado("  No existe registro a modificar...")
                Exit Sub
            End If
            enlazarText()
            vfModificar2 = "actualizar"
            btnModificar.Text = "Actualizar"
            desactivarControles1()
            activarText()
            date1.Focus()
            StatusBarClass.messageBarraEstado("")
            Me.AcceptButton = Me.btnModificar
        Else    'Actualizar
            'validaCampoVacio... creado en el Module ValidarCamposModule.vb, 3=minimo de caractres
            If ValidarCampos() Then
                Exit Sub
            End If

            Me.Refresh()
            Dim finalMytrans As Boolean = False
            Dim myTrans As SqlTransaction = Cn.BeginTransaction()
            Dim wait As New waitForm
            wait.Show()
            Try
                StatusBarClass.messageBarraEstado("  ESPERE POR FAVOR, ACTUALIZANDO INFORMACION....")
                'TPagoDesembolso
                comandoUpdate()
                cmUpdateTable.Transaction = myTrans
                If cmUpdateTable.ExecuteNonQuery() < 1 Then
                    'deshace la transaccion
                    wait.Close()
                    myTrans.Rollback()
                    MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                End If

                Dim codPagD As Integer = BindingSource2.Item(BindingSource2.Position)(0)

                'confirma la transaccion
                myTrans.Commit()    'con exito RAS

                StatusBarClass.messageBarraEstado("  LOS DATOS FUERON GUARDADOS CON EXITO...")
                finalMytrans = True
                vfVan1 = False

                Me.btnCancelar.PerformClick()

                vfVan1 = True
                visualizarDet()

                'Buscando por nombre de campo y luego pocisionarlo con el indice
                BindingSource2.Position = BindingSource2.Find("codPagD", codPagD)

                StatusBarClass.messageBarraEstado("  Registro fué actualizado con exito...")
                wait.Close()
                'colorearFila()
            Catch f As Exception
                wait.Close()
                If finalMytrans Then
                    MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                Else
                    myTrans.Rollback()
                    MessageBox.Show(f.Message & Chr(13) & "NO SE ACTUALIZO EL REGISTRO...PROBLEMAS DE RED...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
            End Try
        End If
    End Sub

    Private Sub enlazarText()
        If BindingSource2.Count = 0 Then
            'desEnlazarText()
        Else
            date1.Value = BindingSource2.Item(BindingSource2.Position)(1)
            cbMod.SelectedValue = BindingSource2.Item(BindingSource2.Position)(6)
            cbCue.SelectedValue = BindingSource2.Item(BindingSource2.Position)(9)
            txtDes.Text = BindingSource2.Item(BindingSource2.Position)(3)
            cbMon.SelectedValue = BindingSource2.Item(BindingSource2.Position)(7)
            txtMon.Text = BindingSource2.Item(BindingSource2.Position)(5)
        End If
    End Sub

    Dim cmUpdateTable As SqlCommand
    Private Sub comandoUpdate()
        cmUpdateTable = New SqlCommand
        cmUpdateTable.CommandType = CommandType.Text
        cmUpdateTable.CommandText = "update TPagoDesembolso set fecPago=@fec,codTipP=@codT,pagoDet=@det,codMon=@codM,montoPago=@monto,idCue=@idC where codPagD=@cod"
        cmUpdateTable.Connection = Cn
        cmUpdateTable.Parameters.Add("@fec", SqlDbType.Date).Value = date1.Value.Date
        cmUpdateTable.Parameters.Add("@codT", SqlDbType.Int, 0).Value = cbMod.SelectedValue
        cmUpdateTable.Parameters.Add("@det", SqlDbType.VarChar, 100).Value = txtDes.Text.Trim()
        cmUpdateTable.Parameters.Add("@codM", SqlDbType.Int, 0).Value = cbMon.SelectedValue
        cmUpdateTable.Parameters.Add("@monto", SqlDbType.Decimal, 0).Value = txtMon.Text.Trim()
        cmUpdateTable.Parameters.Add("@idC", SqlDbType.Int, 0).Value = cbCue.SelectedValue
        cmUpdateTable.Parameters.Add("@cod", SqlDbType.Int, 0).Value = BindingSource2.Item(BindingSource2.Position)(0)
    End Sub
End Class