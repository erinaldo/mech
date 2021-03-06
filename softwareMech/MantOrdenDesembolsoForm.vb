﻿Imports System.Data
Imports System.Data.SqlClient
Imports ComponentesSolucion2008
Public Class MantOrdenDesembolsoForm
    Dim BindingSource0 As New BindingSource
    Dim BindingSource1 As New BindingSource
    Dim BindingSource2 As New BindingSource
    Dim BindingSource3 As New BindingSource
    Dim BindingSource4 As New BindingSource
    Dim BindingSource5 As New BindingSource

    Private Sub MantOrdenDesembolsoForm_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Leave
        Me.Close()
    End Sub

    Private Sub MantOrdenDesembolsoForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Realizando la conexion con SQL Server - ConexionModule.vb
        'conexion() 'active esta linea si desea ejecutar el form independientemente RAS
        'AsignarColoresFormControles()
        VerificaConexion()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        'instanciando los dataAdapter con sus comandos select - DatasetAlmacenModule.vb
        Dim sele As String = "select codIde,razon,ruc,dir,fono,fax,celRpm,email,repres,fono+'  '+fax as fono1,cuentaBan from TIdentidad where estado=1 and idTipId=2" ' '2=proveedor
        crearDataAdapterTable(daTProvee, sele)

        sele = "select distinct codigo,nombre,lugar,color from VLugarTrabajoLogin"
        crearDataAdapterTable(daTUbi, sele)

        vSerie = "002"
        txtSer.Text = "002"
        sele = "select idOP,nroDes,serie,nro,fecDes,est,hist,monto,montoDet,montoDif,estado,codigo,codIde,banco,nroCta,nroDet,datoReq,factCheck,bolCheck,guiaCheck,vouCheck,vouDCheck,reciCheck,otroCheck,descOtro,nroConfor,fecEnt,codMon from VOrdenDesembolso where serie=@ser" 'order by nroDes"
        crearDataAdapterTable(daTabla1, sele)
        daTabla1.SelectCommand.Parameters.Add("@ser", SqlDbType.VarChar, 5).Value = vSerie

        sele = "select codMon,moneda,simbolo from TMoneda"
        crearDataAdapterTable(daTMon, sele)

        sele = "select codPers,codCar,codTipU,nombre,dni,apellido,nom,codPersDes,idOP,estDesem,tipoA,obserDesem from VPersDesem where idOP=@idOP"
        crearDataAdapterTable(daTPers, sele)
        daTPers.SelectCommand.Parameters.Add("@idOP", SqlDbType.Int, 0).Value = 0

        sele = "select codPagD,fecPago,tipoP,pagoDet,simbolo,montoPago,moneda,codTipP,codMon,idOP from VPagoDesembolso where idOP=@idOP"
        crearDataAdapterTable(daTTipo, sele)
        daTTipo.SelectCommand.Parameters.Add("@idOP", SqlDbType.Int, 0).Value = 0

        Try
            'procedimiento para instanciar el dataSet - DatasetAlmacenModule.vb
            crearDSAlmacen()
            'llenat el dataSet con los dataAdapter
            daTProvee.Fill(dsAlmacen, "TIdentidad")
            daTUbi.Fill(dsAlmacen, "VLugarTrabajoLogin")
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")
            daTMon.Fill(dsAlmacen, "TMoneda")
            daTPers.Fill(dsAlmacen, "VPersDesem")
            daTTipo.Fill(dsAlmacen, "VPagoDesembolso")

            BindingSource4.DataSource = dsAlmacen
            BindingSource4.DataMember = "VPersDesem"

            BindingSource1.DataSource = dsAlmacen
            BindingSource1.DataMember = "TIdentidad"
            cbProv.DataSource = BindingSource1
            cbProv.DisplayMember = "razon"
            cbProv.ValueMember = "codIde"
            BindingSource1.Sort = "razon"

            BindingSource2.DataSource = dsAlmacen
            BindingSource2.DataMember = "VLugarTrabajoLogin"
            cbObra.DataSource = BindingSource2
            cbObra.DisplayMember = "nombre"
            cbObra.ValueMember = "codigo"

            BindingSource3.DataSource = dsAlmacen
            BindingSource3.DataMember = "VOrdenDesembolso"
            'Navigator1.BindingSource = BindingSource3
            dgTabla1.DataSource = BindingSource3
            'dgTabla1.SelectionMode = DataGridViewSelectionMode.FullRowSelect 'Seleccionar fila completa
            BindingSource3.Sort = "nroDes"
            ModificarColumnasDGV()

            BindingSource0.DataSource = dsAlmacen
            BindingSource0.DataMember = "TMoneda"
            cbMon.DataSource = BindingSource0
            cbMon.DisplayMember = "simbolo"
            cbMon.ValueMember = "codMon"

            BindingSource5.DataSource = dsAlmacen
            BindingSource5.DataMember = "VPagoDesembolso"

            configurarColorControl()

            recuperarUltimoNro(vSerie)

            crearColumnaListView1()
            vfVan1 = True   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = True
            leerProvee()
            enlazarText()

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

    Private Sub MantOrdenDesembolsoForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        colorearFila()
    End Sub

    Private Sub colorearFila()
        For j As Short = 0 To BindingSource3.Count - 1
            If BindingSource3.Item(j)(10) = 1 Then 'Terminado
                dgTabla1.Rows(j).Cells(5).Style.BackColor = Color.Green
            End If
            If BindingSource3.Item(j)(10) = 2 Then 'Cerrado
                dgTabla1.Rows(j).Cells(5).Style.BackColor = Color.AliceBlue
            End If
            If BindingSource3.Item(j)(10) = 3 Then 'Anulado
                dgTabla1.Rows(j).Cells(5).Style.BackColor = Color.Yellow
            End If
        Next
    End Sub

    Private Sub cbProv_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbProv.SelectedIndexChanged
        leerProvee()
    End Sub

    Private Sub leerProvee()
        If BindingSource1.Position <> -1 Then
            If vfVan1 Then
                txtRuc.Text = BindingSource1.Item(cbProv.SelectedIndex)(2)
                txtFono.Text = BindingSource1.Item(cbProv.SelectedIndex)(9)
                txtEma.Text = BindingSource1.Item(cbProv.SelectedIndex)(7)
                txtNroCta.Text = BindingSource1.Item(cbProv.SelectedIndex)(10)
            End If
        End If
    End Sub

    Private Sub ModificarColumnasDGV()
        With dgTabla1
            .Columns(0).Visible = False
            .Columns(1).Visible = False
            .Columns(2).HeaderText = "Serie"
            .Columns(2).Width = 40
            .Columns(3).HeaderText = "NºOrd."
            .Columns(3).Width = 50
            .Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Columns(4).HeaderText = "Fec_Ord"
            .Columns(4).Width = 70
            .Columns(5).Width = 75
            .Columns(5).HeaderText = "Estado"
            .Columns(6).Visible = False
            .Columns(6).Width = 800
            .Columns(6).HeaderText = ""
            .Columns(7).Visible = False
            .Columns(8).Visible = False
            .Columns(9).Visible = False
            .Columns(10).Visible = False
            .Columns(11).Visible = False
            .Columns(12).Visible = False
            .Columns(13).Visible = False
            .Columns(14).Visible = False
            .Columns(15).Visible = False
            .Columns(16).Visible = False
            .Columns(17).Visible = False
            .Columns(18).Visible = False
            .Columns(19).Visible = False
            .Columns(20).Visible = False
            .Columns(21).Visible = False
            .Columns(22).Visible = False
            .Columns(23).Visible = False
            .Columns(24).Visible = False
            .Columns(25).Visible = False
            .Columns(26).Visible = False
            .Columns(27).Visible = False
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
        Label2.ForeColor = ForeColorLabel
        Label3.ForeColor = ForeColorLabel
        Label4.ForeColor = ForeColorLabel
        Label5.ForeColor = ForeColorLabel
        Label6.ForeColor = ForeColorLabel
        Label7.ForeColor = ForeColorLabel
        Label8.ForeColor = ForeColorLabel
        Label9.ForeColor = ForeColorLabel
        Label10.ForeColor = ForeColorLabel
        Label11.ForeColor = ForeColorLabel
        Label12.ForeColor = ForeColorLabel
        Label13.ForeColor = ForeColorLabel
        Label14.ForeColor = ForeColorLabel
        Label15.ForeColor = ForeColorLabel
        Label16.ForeColor = ForeColorLabel
        Label17.ForeColor = ForeColorLabel
        Label18.ForeColor = ForeColorLabel
        Label19.ForeColor = ForeColorLabel
        Label29.ForeColor = ForeColorLabel
        Label30.ForeColor = ForeColorLabel
        Label31.ForeColor = ForeColorLabel
        Label32.ForeColor = ForeColorLabel
        Label33.ForeColor = ForeColorLabel
        Label34.ForeColor = ForeColorLabel
        Label35.ForeColor = ForeColorLabel
        Label37.ForeColor = ForeColorLabel
        Label38.ForeColor = ForeColorLabel
        Label39.ForeColor = ForeColorLabel
        Label40.ForeColor = ForeColorLabel
        Label42.ForeColor = ForeColorLabel
        Label43.ForeColor = ForeColorLabel
        Label44.ForeColor = ForeColorLabel
        Label45.ForeColor = ForeColorLabel
        Label46.ForeColor = ForeColorLabel
        Label47.ForeColor = ForeColorLabel
        Label48.ForeColor = ForeColorLabel
        Label49.ForeColor = ForeColorLabel
        Label50.ForeColor = ForeColorLabel
        checkB1.ForeColor = ForeColorLabel
        checkB2.ForeColor = ForeColorLabel
        checkB3.ForeColor = ForeColorLabel
        checkB4.ForeColor = ForeColorLabel
        checkB5.ForeColor = ForeColorLabel
        checkB6.ForeColor = ForeColorLabel
        checkB7.ForeColor = ForeColorLabel
        btnAperturar.ForeColor = ForeColorButtom
        btnAperturar1.ForeColor = ForeColorButtom
        btnModificar.ForeColor = ForeColorButtom
        btnElimina.ForeColor = ForeColorButtom
        btnCerrar.ForeColor = ForeColorButtom
        btnOrden.ForeColor = ForeColorButtom
    End Sub

    Private Sub btnCerrar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCerrar.Click
        dgTabla1.Dispose()
        Me.Close()
    End Sub

    Private Sub recuperarUltimoNro(ByVal serie As String)
        Dim cmdMaxCodigo As SqlCommand = New SqlCommand
        cmdMaxCodigo.CommandType = CommandType.Text
        cmdMaxCodigo.CommandText = "select isnull(max(nroDes),0)+1 from TOrdenDesembolso where serie='" & serie & "'"
        cmdMaxCodigo.Connection = Cn
        asignarNro(cmdMaxCodigo.ExecuteScalar)
    End Sub

    Private Sub asignarNro(ByVal max As Integer)
        Select Case CInt(max)
            Case Is < 99
                txtNro.Text = "000" & max
            Case 100 To 999
                txtNro.Text = "00" & max
            Case 1000 To 9999
                txtNro.Text = "0" & max
            Case Is > 9999
                txtNro.Text = max
        End Select
    End Sub

    Private Function recuperarNroOrdenCompra(ByVal idOP As Integer) As String
        Dim cmdCampo As SqlCommand = New SqlCommand
        cmdCampo.CommandType = CommandType.Text
        cmdCampo.CommandText = "select mech.FN_ConcaNroOrden1(" & idOP & ")"
        cmdCampo.Connection = Cn
        Return cmdCampo.ExecuteScalar
    End Function

    Private Sub dgTabla1_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgTabla1.CurrentCellChanged
        enlazarText()
    End Sub

    Dim vfVan2 As Boolean = False
    Private Sub enlazarText()
        If vfVan2 Then
            Me.Cursor = Cursors.WaitCursor
            If BindingSource3.Count = 0 Then
                'desEnlazarText()
            Else
                date1.Value = BindingSource3.Item(BindingSource3.Position)(4)
                txtMon.Text = BindingSource3.Item(BindingSource3.Position)(7)
                txtDet.Text = BindingSource3.Item(BindingSource3.Position)(8)
                txtTot.Text = BindingSource3.Item(BindingSource3.Position)(9)
                cbMon.SelectedValue = BindingSource3.Item(BindingSource3.Position)(27)
                cambiarNroTotalLetra()
                cbObra.SelectedValue = BindingSource3.Item(BindingSource3.Position)(11)
                cbProv.SelectedValue = BindingSource3.Item(BindingSource3.Position)(12)
                txtBan.Text = BindingSource3.Item(BindingSource3.Position)(13)
                txtNroCta.Text = BindingSource3.Item(BindingSource3.Position)(14)
                txtNroDet.Text = BindingSource3.Item(BindingSource3.Position)(15)
                txtOrden.Text = recuperarNroOrdenCompra(BindingSource3.Item(BindingSource3.Position)(0)).Trim()
                txtDato.Text = BindingSource3.Item(BindingSource3.Position)(16)

                txtOtro.Text = BindingSource3.Item(BindingSource3.Position)(24)
                txtNroCon.Text = BindingSource3.Item(BindingSource3.Position)(25)
                txtFec.Text = BindingSource3.Item(BindingSource3.Position)(26)

                If BindingSource3.Item(BindingSource3.Position)(17) = 1 Then 'Fact check
                    checkB1.Checked = True
                Else 'NO chekeado
                    checkB1.Checked = False
                End If
                If BindingSource3.Item(BindingSource3.Position)(18) = 1 Then 'Boleta check
                    checkB2.Checked = True
                Else 'NO chekeado
                    checkB2.Checked = False
                End If
                If BindingSource3.Item(BindingSource3.Position)(19) = 1 Then 'guia remision check
                    checkB3.Checked = True
                Else 'NO chekeado
                    checkB3.Checked = False
                End If
                If BindingSource3.Item(BindingSource3.Position)(22) = 1 Then 'Recibo check
                    checkB4.Checked = True
                Else 'NO chekeado
                    checkB4.Checked = False
                End If
                If BindingSource3.Item(BindingSource3.Position)(20) = 1 Then 'Voucher check
                    checkB5.Checked = True
                Else 'NO chekeado
                    checkB5.Checked = False
                End If
                If BindingSource3.Item(BindingSource3.Position)(21) = 1 Then 'Voucher detraccion check
                    checkB6.Checked = True
                Else 'NO chekeado
                    checkB6.Checked = False
                End If
                If BindingSource3.Item(BindingSource3.Position)(23) = 1 Then 'Otro check
                    checkB7.Checked = True
                Else 'NO chekeado
                    checkB7.Checked = False
                End If

                selePersDesem() 'select a todo el personal que esta autorizando

                Dim estado As Integer
                'extraer a SOLICITANTE
                BindingSource4.Filter = "tipoA=1" '1=solicitante
                If BindingSource4.Position <> -1 Then
                    txtNom1.Text = BindingSource4.Item(BindingSource4.Position)(6)
                    txtDni1.Text = BindingSource4.Item(BindingSource4.Position)(4)
                    txtObs1.Text = BindingSource4.Item(BindingSource4.Position)(11)
                    estado = BindingSource4.Item(BindingSource4.Position)(9)
                    If estado = 1 Then 'aprobado
                        cbF1.SelectedIndex = 0
                    End If
                    If estado = 2 Then 'Observado
                        cbF1.SelectedIndex = 1
                    End If
                    If estado = 3 Then 'denegado
                        cbF1.SelectedIndex = 2
                    End If
                End If

                'extraer a GERENTE
                BindingSource4.Filter = "tipoA=2" '2=Gerencia
                If BindingSource4.Position <> -1 Then
                    txtNom2.Text = BindingSource4.Item(BindingSource4.Position)(6)
                    txtDni2.Text = BindingSource4.Item(BindingSource4.Position)(4)
                    txtObs2.Text = BindingSource4.Item(BindingSource4.Position)(11)
                    estado = BindingSource4.Item(BindingSource4.Position)(9)
                    If estado = 1 Then 'aprobado
                        cbF2.SelectedIndex = 0
                    End If
                    If estado = 2 Then 'Observado
                        cbF2.SelectedIndex = 1
                    End If
                    If estado = 3 Then 'denegado
                        cbF2.SelectedIndex = 2
                    End If
                Else
                    cbF2.BackColor = Color.FromName(Color.White.Name)
                    cbF2.SelectedIndex = -1

                    txtNom2.Clear()
                    txtDni2.Clear()
                    txtObs2.Clear()
                End If

                'extraer a TESORERIA
                BindingSource4.Filter = "tipoA=3" '3=Tesoreria
                If BindingSource4.Position <> -1 Then
                    txtNom3.Text = BindingSource4.Item(BindingSource4.Position)(6)
                    txtDni3.Text = BindingSource4.Item(BindingSource4.Position)(4)
                    txtObs3.Text = BindingSource4.Item(BindingSource4.Position)(11)
                    estado = BindingSource4.Item(BindingSource4.Position)(9)
                    If estado = 1 Then 'aprobado
                        cbF3.SelectedIndex = 0
                    End If
                    If estado = 2 Then 'Observado
                        cbF3.SelectedIndex = 1
                    End If
                    If estado = 3 Then 'denegado
                        cbF3.SelectedIndex = 2
                    End If
                Else
                    cbF3.BackColor = Color.FromName(Color.White.Name)
                    cbF3.SelectedIndex = -1

                    txtNom3.Clear()
                    txtDni3.Clear()
                    txtObs3.Clear()
                End If

                'extraer a CONTABILIDAD
                BindingSource4.Filter = "tipoA=4" '4=Contabilidad
                If BindingSource4.Position <> -1 Then
                    txtNom4.Text = BindingSource4.Item(BindingSource4.Position)(6)
                    txtDni4.Text = BindingSource4.Item(BindingSource4.Position)(4)
                    txtObs4.Text = BindingSource4.Item(BindingSource4.Position)(11)
                    estado = BindingSource4.Item(BindingSource4.Position)(9)
                    If estado = 1 Then 'aprobado
                        cbF4.SelectedIndex = 0
                    End If
                    If estado = 2 Then 'Observado
                        cbF4.SelectedIndex = 1
                    End If
                    If estado = 3 Then 'denegado
                        cbF4.SelectedIndex = 2
                    End If
                Else
                    cbF4.BackColor = Color.FromName(Color.White.Name)
                    cbF4.SelectedIndex = -1

                    txtNom4.Clear()
                    txtDni4.Clear()
                    txtObs4.Clear()
                End If

                selePagoDesem()
                listar1()

                Me.Cursor = Cursors.Default
            End If
        End If
    End Sub

    Dim vfVan1 As Boolean = False
    Private Sub selePersDesem()
        If vfVan1 Then
            If BindingSource3.Position = -1 Then
                Exit Sub
            End If
            Me.Cursor = Cursors.WaitCursor
            dsAlmacen.Tables("VPersDesem").Clear()
            daTPers.SelectCommand.Parameters("@idOP").Value = BindingSource3.Item(BindingSource3.Position)(0)
            daTPers.Fill(dsAlmacen, "VPersDesem")
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub selePagoDesem()
        If vfVan1 Then
            If BindingSource3.Position = -1 Then
                Exit Sub
            End If
            Me.Cursor = Cursors.WaitCursor
            dsAlmacen.Tables("VPagoDesembolso").Clear()
            daTTipo.SelectCommand.Parameters("@idOP").Value = BindingSource3.Item(BindingSource3.Position)(0)
            daTTipo.Fill(dsAlmacen, "VPagoDesembolso")
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub crearColumnaListView1()
        With ListView1
            .Columns.Add("Fecha", 90, HorizontalAlignment.Left)
            .Columns.Add("Tipo", 80, HorizontalAlignment.Left)
            .Columns.Add("Detalle", 260, HorizontalAlignment.Left)
            .Columns.Add("", 50, HorizontalAlignment.Left)
            .Columns.Add("Monto", 100, HorizontalAlignment.Left)
            .View = View.Details
            .FullRowSelect = True   'Select filas completa
            .GridLines = True
            .HeaderStyle = ColumnHeaderStyle.Nonclickable    'Estilo de encabezado
        End With
    End Sub

    Private Sub listar1()
        ListView1.Items.Clear()
        Dim fila As ListViewItem
        For i As Short = 0 To BindingSource5.Count - 1
            fila = ListView1.Items.Add(BindingSource5.Item(i)(1))
            fila.SubItems.Add(BindingSource5.Item(i)(2))
            fila.SubItems.Add(BindingSource5.Item(i)(3))
            fila.SubItems.Add(BindingSource5.Item(i)(4))
            fila.SubItems.Add(BindingSource5.Item(i)(5))
        Next
    End Sub

    Private Sub cambiarNroTotalLetra()
        Dim cALetra As New Num2LetEsp  'clase definida por el usuario
        If cbMon.SelectedValue = 30 Then    '30=Nuevos solesl
            'If BindingSource10.Item(BindingSource10.Position)(7) = 30 Then
            cALetra.Moneda = "Nuevos Soles"
        Else    'dolares
            cALetra.Moneda = "Dólares Americanos"
        End If
        'Inicia el Proceso para identificar la cantidad a convertir
        If Val(txtMon.Text) > 0 Then
            cALetra.Numero = Val(CDbl(txtMon.Text))
            txtLetraTotal.Text = "SON: " & cALetra.ALetra.ToUpper()
        End If
    End Sub

    Private Sub cbF1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbF1.SelectedIndexChanged
        If cbF1.SelectedIndex = 0 Then 'Aprobado
            cbF1.BackColor = Color.FromName(Color.Green.Name)
        End If
        If cbF1.SelectedIndex = 1 Then 'Observado
            cbF1.BackColor = Color.FromName(Color.Yellow.Name)
        End If
        If cbF1.SelectedIndex = 2 Then 'Denegado
            cbF1.BackColor = Color.FromName(Color.Red.Name)
        End If
        btnF1.Focus()
    End Sub

    Private Sub cbF2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbF2.SelectedIndexChanged
        If cbF2.SelectedIndex = 0 Then 'Aprobado
            cbF2.BackColor = Color.FromName(Color.Green.Name)
        End If
        If cbF2.SelectedIndex = 1 Then 'Observado
            cbF2.BackColor = Color.FromName(Color.Yellow.Name)
        End If
        If cbF2.SelectedIndex = 2 Then 'Denegado
            cbF2.BackColor = Color.FromName(Color.Red.Name)
        End If
        'btnF2.Focus()
    End Sub

    Private Sub cbF3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbF3.SelectedIndexChanged
        If cbF3.SelectedIndex = 0 Then 'Aprobado
            cbF3.BackColor = Color.FromName(Color.Green.Name)
        End If
        If cbF3.SelectedIndex = 1 Then 'Observado
            cbF3.BackColor = Color.FromName(Color.Yellow.Name)
        End If
        If cbF3.SelectedIndex = 2 Then 'Denegado
            cbF3.BackColor = Color.FromName(Color.Red.Name)
        End If
        'btnF3.Focus()
    End Sub

    Private Sub cbF4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbF4.SelectedIndexChanged
        If cbF4.SelectedIndex = 0 Then 'Aprobado
            cbF4.BackColor = Color.FromName(Color.Green.Name)
        End If
        If cbF4.SelectedIndex = 1 Then 'Observado
            cbF4.BackColor = Color.FromName(Color.Yellow.Name)
        End If
        If cbF4.SelectedIndex = 2 Then 'Denegado
            cbF4.BackColor = Color.FromName(Color.Red.Name)
        End If
        'btnF4.Focus()
    End Sub

    Private Sub checkB7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkB7.CheckedChanged
        txtOtro.Focus()
        txtOtro.SelectAll()
    End Sub

    Private Function recuperarCodPers(ByVal codPersDes As Integer) As Integer
        Dim cmdCampo As SqlCommand = New SqlCommand
        cmdCampo.CommandType = CommandType.Text
        cmdCampo.CommandText = "select isnull(max(codPers),0) from TPersDesem where codPersDes=" & codPersDes
        cmdCampo.Connection = Cn
        Return cmdCampo.ExecuteScalar
    End Function

    Private Function recuperarCodPersDesem(ByVal idOp As Integer, ByVal tipo As Short) As Integer
        Dim cmdCampo As SqlCommand = New SqlCommand
        cmdCampo.CommandType = CommandType.Text
        cmdCampo.CommandText = "select isnull(max(codPersDes),0) from TPersDesem where idOP=" & idOp & " and tipoA=" & tipo
        cmdCampo.Connection = Cn
        Return cmdCampo.ExecuteScalar
    End Function

    Dim cmUpdateTable3 As SqlCommand
    Private Sub comandoUpdate3(ByVal estado As Short, ByVal obs As String, ByVal codPersDes As Integer)
        cmUpdateTable3 = New SqlCommand
        cmUpdateTable3.CommandType = CommandType.Text
        cmUpdateTable3.CommandText = "update TPersDesem set estDesem=@est,obserDesem=@obs where codPersDes=@cod"
        cmUpdateTable3.Connection = Cn
        cmUpdateTable3.Parameters.Add("@est", SqlDbType.Int, 0).Value = estado
        cmUpdateTable3.Parameters.Add("@obs", SqlDbType.VarChar, 100).Value = obs
        cmUpdateTable3.Parameters.Add("@cod", SqlDbType.Int, 0).Value = codPersDes
    End Sub

    Private Sub btnF1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnF1.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado(" No existe Orden de Desembolso a procesar...")
            Exit Sub
        End If

        If cbF1.SelectedIndex = -1 Then
            MessageBox.Show("Seleccione Opción valida...", nomNegocio, Nothing, MessageBoxIcon.Exclamation)
            cbF1.Focus()
            Exit Sub
        End If

        Dim codPersDes As Integer = recuperarCodPersDesem(BindingSource3.Item(BindingSource3.Position)(0), 1) '1=Solicitante
        If codPersDes > 0 Then 'Existe firma
            If recuperarCodPers(codPersDes) <> vPass Then 'Usurio no es de dirma inicial
                MessageBox.Show("Proceso Denegado, Usuario no es de la Firma Inicial...", nomNegocio, Nothing, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de seleccionar opción: " & cbF1.Text.Trim() & " para Orden de Desembolso", nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        Dim comentario As New CometarioForm
        comentario.ShowDialog()

        recuperarUltimoNro(vSerie)
        Dim campo As Integer = CInt(txtNro.Text)

        Dim finalMytrans As Boolean = False
        Dim wait As New waitForm
        Me.Cursor = Cursors.WaitCursor
        wait.Show()
        'estableciendo una transaccion
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Try
            StatusBarClass.messageBarraEstado("  PROCESANDO DATOS...")
            Me.Refresh()
            Dim opcion As Short
            If cbF1.SelectedIndex = 0 Then
                opcion = 1 'Aprobado
            End If
            If cbF1.SelectedIndex = 1 Then
                opcion = 2 'Observado
            End If
            If cbF1.SelectedIndex = 2 Then
                opcion = 3 'denegado
            End If

            If codPersDes = 0 Then 'no existe firma insertar
                'TPersDesem
                comandoInsert2(BindingSource3.Item(BindingSource3.Position)(0), vPass, opcion, 1, vObs, date1.Value.Date)  '1=solicitante
                cmInserTable2.Transaction = myTrans
                If cmInserTable2.ExecuteNonQuery() < 1 Then
                    wait.Close()
                    Me.Cursor = Cursors.Default
                    myTrans.Rollback()
                    MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
            Else 'existe firma actualizar
                'TPersDesem
                comandoUpdate3(opcion, vObs, codPersDes)
                cmUpdateTable3.Transaction = myTrans
                If cmUpdateTable3.ExecuteNonQuery() < 1 Then
                    'deshace la transaccion
                    wait.Close()
                    Me.Cursor = Cursors.Default
                    myTrans.Rollback()
                    MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
            End If

            Dim idOP As Integer = BindingSource3.Item(BindingSource3.Position)(0)
            'confirma la transaccion
            myTrans.Commit()    'con exito RAS

            StatusBarClass.messageBarraEstado("  LOS DATOS FUERON GUARDADOS CON EXITO...")
            finalMytrans = True
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.Position = BindingSource3.Find("idOP", idOP)

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué procesado con exito...")

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            If finalMytrans Then
                MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            Else
                myTrans.Rollback()
                MessageBox.Show(f.Message & Chr(13) & "NO SE GUARDO EL REGISTRO...PROBLEMAS DE RED...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If
        End Try
    End Sub

    Private Function ValidarCampos() As Boolean
        'Todas las funciones estan creadas en el module ValidarCamposModule.vb
        If ValidarCantMayorCero(txtMon.Text) Then
            txtMon.errorProv()
            Return True
        End If
        If ValidaNroMayorOigualCero(txtDet.Text) Then
            txtDet.errorProv()
            Return True
        End If
        If ValidaNroMayorOigualCero(txtTot.Text) Then
            txtTot.errorProv()
            Return True
        End If
        'Todo OK RAS
        Return False
    End Function

    Private Sub txtMon_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMon.TextChanged, txtDet.TextChanged
        If Not IsNumeric(txtDet.Text) Or Not IsNumeric(txtMon.Text) Then
            Exit Sub
        End If
        Dim cero As Double = CDbl(txtDet.Text)
        If cero = 0 Then
            txtTot.Text = "0"
        Else
            If cero > 0 Then
                txtTot.Text = txtMon.Text - txtDet.Text
            Else
                txtTot.Text = "0"
            End If
        End If
    End Sub

    Private Sub btnAperturar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAperturar.Click
        If ValidaFechaMayorXXXX(date1.Value.Date, 2013) Then
            MessageBox.Show("Ingrese fecha mayor al año 2012", nomNegocio, Nothing, MessageBoxIcon.Asterisk)
            Exit Sub
        End If

        If ValidarCampos() Then
            Exit Sub
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de aperturar ORDEN de DESEMBOLSO" & Chr(13) & "Serie: " & txtSer.Text & "  Nº " & txtNro.Text.Trim(), nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        recuperarUltimoNro(vSerie)
        Dim campo As Integer = CInt(txtNro.Text)

        Dim finalMytrans As Boolean = False
        Dim wait As New waitForm
        Me.Cursor = Cursors.WaitCursor
        wait.Show()
        'estableciendo una transaccion
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Try
            StatusBarClass.messageBarraEstado("  PROCESANDO DATOS...")
            Me.Refresh()

            'TOrdenDesembolso
            comandoInsert1()
            cmInserTable1.Transaction = myTrans
            If cmInserTable1.ExecuteNonQuery() < 1 Then
                wait.Close()
                Me.Cursor = Cursors.Default
                myTrans.Rollback()
                MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If
            Dim idOP As Integer = cmInserTable1.Parameters("@Identity").Value

            'TPersDesem
            comandoInsert2(idOP, vPass, 1, 1, txtObs1.Text.Trim(), date1.Value.Date)  '1=aprobado  1=solicitante
            cmInserTable2.Transaction = myTrans
            If cmInserTable2.ExecuteNonQuery() < 1 Then
                wait.Close()
                Me.Cursor = Cursors.Default
                myTrans.Rollback()
                MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If

            'confirma la transaccion
            myTrans.Commit()    'con exito RAS

            StatusBarClass.messageBarraEstado("  LOS DATOS FUERON GUARDADOS CON EXITO...")
            finalMytrans = True
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.Position = BindingSource3.Find("idOP", idOP)

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué guardado con exito...")

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            colorearFila()

            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            If finalMytrans Then
                MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            Else
                myTrans.Rollback()
                MessageBox.Show(f.Message & Chr(13) & "NO SE GUARDO EL REGISTRO...PROBLEMAS DE RED...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If
        End Try
    End Sub

    Dim cmInserTable1 As SqlCommand
    Private Sub comandoInsert1()
        cmInserTable1 = New SqlCommand
        cmInserTable1.CommandType = CommandType.StoredProcedure
        cmInserTable1.CommandText = "PA_InsertOrdenDesembolso"
        cmInserTable1.Connection = Cn
        cmInserTable1.Parameters.Add("@ser", SqlDbType.VarChar, 5).Value = txtSer.Text.Trim()
        cmInserTable1.Parameters.Add("@nroD", SqlDbType.Int, 0).Value = txtNro.Text
        cmInserTable1.Parameters.Add("@fecD", SqlDbType.Date).Value = date1.Value.Date
        cmInserTable1.Parameters.Add("@codMon", SqlDbType.Int, 0).Value = cbMon.SelectedValue
        cmInserTable1.Parameters.Add("@mon", SqlDbType.Decimal, 0).Value = txtMon.Text
        cmInserTable1.Parameters.Add("@mon1", SqlDbType.Decimal, 0).Value = txtDet.Text
        cmInserTable1.Parameters.Add("@mon2", SqlDbType.Decimal, 0).Value = txtTot.Text
        cmInserTable1.Parameters.Add("@est", SqlDbType.Int, 0).Value = 0   'pendiente
        cmInserTable1.Parameters.Add("@cod", SqlDbType.VarChar, 10).Value = cbObra.SelectedValue 'vSCodigo
        cmInserTable1.Parameters.Add("@codIde", SqlDbType.Int, 0).Value = cbProv.SelectedValue
        cmInserTable1.Parameters.Add("@ban", SqlDbType.VarChar, 30).Value = txtBan.Text.Trim()
        cmInserTable1.Parameters.Add("@nroC", SqlDbType.VarChar, 50).Value = txtNroCta.Text.Trim()
        cmInserTable1.Parameters.Add("@nroDE", SqlDbType.VarChar, 30).Value = txtNroDet.Text.Trim()
        cmInserTable1.Parameters.Add("@dato", SqlDbType.VarChar, 200).Value = txtDato.Text.Trim()

        If checkB1.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@fact", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@fact", SqlDbType.Int, 0).Value = 0
        End If

        If checkB2.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@bol", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@bol", SqlDbType.Int, 0).Value = 0
        End If

        If checkB3.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@guia", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@guia", SqlDbType.Int, 0).Value = 0
        End If

        If checkB5.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@vou", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@vou", SqlDbType.Int, 0).Value = 0
        End If

        If checkB6.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@vouD", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@vouD", SqlDbType.Int, 0).Value = 0
        End If

        If checkB4.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@reci", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@reci", SqlDbType.Int, 0).Value = 0
        End If

        If checkB7.Checked Then 'Chekeado = 1
            cmInserTable1.Parameters.Add("@otro", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmInserTable1.Parameters.Add("@otro", SqlDbType.Int, 0).Value = 0
        End If
        cmInserTable1.Parameters.Add("@des", SqlDbType.VarChar, 60).Value = txtOtro.Text.Trim()

        cmInserTable1.Parameters.Add("@nroCF", SqlDbType.VarChar, 30).Value = "" 'txtNroCon.Text.Trim()
        cmInserTable1.Parameters.Add("@fec", SqlDbType.VarChar, 10).Value = "" 'txtFec.Text.Trim()
        cmInserTable1.Parameters.Add("@hist", SqlDbType.VarChar, 200).Value = "Aperturo " & Now.Date & " " & vPass & "-" & vSUsuario
        cmInserTable1.Parameters.Add("@codSerO", SqlDbType.Int, 0).Value = 1   'CodSerie 002
        'configurando direction output = parametro de solo salida
        cmInserTable1.Parameters.Add("@Identity", SqlDbType.Int, 0)
        cmInserTable1.Parameters("@Identity").Direction = ParameterDirection.Output
    End Sub

    Dim cmInserTable2 As SqlCommand
    Private Sub comandoInsert2(ByVal idOP As Integer, ByVal codPers As Integer, ByVal estado As Integer, ByVal tipo As Integer, ByVal obs As String, ByVal fecha As String)
        cmInserTable2 = New SqlCommand
        cmInserTable2.CommandType = CommandType.Text
        cmInserTable2.CommandText = "insert into TPersDesem(idOP,codPers,estDesem,tipoA,obserDesem,fecFir) values(@id,@codP,@est,@tipo,@obs,@fec)"
        cmInserTable2.Connection = Cn
        cmInserTable2.Parameters.Add("@id", SqlDbType.Int, 0).Value = idOP
        cmInserTable2.Parameters.Add("@codP", SqlDbType.Int, 0).Value = codPers 'vPass
        cmInserTable2.Parameters.Add("@est", SqlDbType.Int, 0).Value = estado '1=Aprobado
        cmInserTable2.Parameters.Add("@tipo", SqlDbType.Int, 0).Value = tipo '1=Solicitante
        cmInserTable2.Parameters.Add("@obs", SqlDbType.VarChar, 100).Value = obs
        cmInserTable2.Parameters.Add("@fec", SqlDbType.Date).Value = fecha
    End Sub

    Private Sub btnModificar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnModificar.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado("  No existe Orden de Desembolso a actualizar...")
            Exit Sub
        End If

        If ValidaFechaMayorXXXX(date1.Value.Date, 2013) Then
            MessageBox.Show("Ingrese fecha mayor al año 2012", nomNegocio, Nothing, MessageBoxIcon.Asterisk)
            date1.Focus()
            Exit Sub
        End If

        If ValidarCampos() Then
            Exit Sub
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de GUARDAR MODIFICACIONES en Orden de Desembolso" & Chr(13) & "Serie: " & BindingSource3.Item(BindingSource3.Position)(2) & "  Nº " & BindingSource3.Item(BindingSource3.Position)(3), nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        Me.Refresh()
        Dim finalMytrans As Boolean = False
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        Try
            StatusBarClass.messageBarraEstado("  ESPERE POR FAVOR, GUARDANDO INFORMACION....")
            Dim idOP As Integer = BindingSource3.Item(BindingSource3.Position)(0)
            'TOrdenDesembolso
            comandoUpdate1()
            cmUpdateTable1.Transaction = myTrans
            If cmUpdateTable1.ExecuteNonQuery() < 1 Then
                'deshace la transaccion
                wait.Close()
                Me.Cursor = Cursors.Default
                myTrans.Rollback()
                MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
            End If

            'confirma la transaccion
            myTrans.Commit()    'con exito RAS
            finalMytrans = True
            StatusBarClass.messageBarraEstado("  LOS DATOS FUERON ACTUALIZADOS CON EXITO...")
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.Position = BindingSource3.Find("idOP", idOP)

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué guardado con exito...")

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            colorearFila()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué actualizado con exito...")
            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
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
    End Sub

    Dim cmUpdateTable1 As SqlCommand
    Private Sub comandoUpdate1()
        cmUpdateTable1 = New SqlCommand
        cmUpdateTable1.CommandType = CommandType.Text
        cmUpdateTable1.CommandText = "update TOrdenDesembolso set fecDes=@fecD,codMon=@codMon,monto=@mon,montoDet=@mon1,montoDif=@mon2,codigo=@cod,codIde=@codIde,banco=@ban,nroCta=@nroC,nroDet=@nroDE,datoReq=@dato,factCheck=@fact,bolCheck=@bol,guiaCheck=@guia,vouCheck=@vou,vouDCheck=@vouD,reciCheck=@reci,otroCheck=@otro,descOtro=@des,hist=@hist where idOP=@idOP"
        cmUpdateTable1.Connection = Cn
        cmUpdateTable1.Parameters.Add("@fecD", SqlDbType.Date).Value = date1.Value.Date
        cmUpdateTable1.Parameters.Add("@codMon", SqlDbType.Int, 0).Value = cbMon.SelectedValue
        cmUpdateTable1.Parameters.Add("@mon", SqlDbType.Decimal, 0).Value = txtMon.Text
        cmUpdateTable1.Parameters.Add("@mon1", SqlDbType.Decimal, 0).Value = txtDet.Text
        cmUpdateTable1.Parameters.Add("@mon2", SqlDbType.Decimal, 0).Value = txtTot.Text
        cmUpdateTable1.Parameters.Add("@cod", SqlDbType.VarChar, 10).Value = cbObra.SelectedValue 'vSCodigo
        cmUpdateTable1.Parameters.Add("@codIde", SqlDbType.Int, 0).Value = cbProv.SelectedValue
        cmUpdateTable1.Parameters.Add("@ban", SqlDbType.VarChar, 30).Value = txtBan.Text.Trim()
        cmUpdateTable1.Parameters.Add("@nroC", SqlDbType.VarChar, 50).Value = txtNroCta.Text.Trim()
        cmUpdateTable1.Parameters.Add("@nroDE", SqlDbType.VarChar, 30).Value = txtNroDet.Text.Trim()
        cmUpdateTable1.Parameters.Add("@dato", SqlDbType.VarChar, 200).Value = txtDato.Text.Trim()

        If checkB1.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@fact", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@fact", SqlDbType.Int, 0).Value = 0
        End If

        If checkB2.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@bol", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@bol", SqlDbType.Int, 0).Value = 0
        End If

        If checkB3.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@guia", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@guia", SqlDbType.Int, 0).Value = 0
        End If

        If checkB5.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@vou", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@vou", SqlDbType.Int, 0).Value = 0
        End If

        If checkB6.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@vouD", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@vouD", SqlDbType.Int, 0).Value = 0
        End If

        If checkB4.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@reci", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@reci", SqlDbType.Int, 0).Value = 0
        End If

        If checkB7.Checked Then 'Chekeado = 1
            cmUpdateTable1.Parameters.Add("@otro", SqlDbType.Int, 0).Value = 1
        Else 'No Chekeado
            cmUpdateTable1.Parameters.Add("@otro", SqlDbType.Int, 0).Value = 0
        End If
        cmUpdateTable1.Parameters.Add("@des", SqlDbType.VarChar, 60).Value = txtOtro.Text.Trim()
        cmUpdateTable1.Parameters.Add("@hist", SqlDbType.VarChar, 200).Value = BindingSource3.Item(BindingSource3.Position)(6) & "  Modifico " & Now.Date & " " & vPass & "-" & vSUsuario
        cmUpdateTable1.Parameters.Add("@idOP", SqlDbType.Int, 0).Value = BindingSource3.Item(BindingSource3.Position)(0)
    End Sub

    Private Sub btnOrden_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOrden.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado(" No existe Orden de Desembolso a procesar...")
            Exit Sub
        End If

        vCod1 = BindingSource3.Item(BindingSource3.Position)(2) & " - " & BindingSource3.Item(BindingSource3.Position)(3)
        vNroOrden = BindingSource3.Item(BindingSource3.Position)(0) 'idOP para retorno
        vCod2 = "0"  'retornar el NROORDEN
        vCod3 = txtOrden.Text.Trim() 'pa comparar si ya tiene orden de compra ya anexado

        Dim jala As New jalarOrdenCompraForm
        jala.ShowDialog()

        If CInt(vCod2) > 0 Then
            txtOrden.Text = recuperarNroOrden(BindingSource3.Item(BindingSource3.Position)(0))
        Else

        End If
    End Sub

    Private Function recuperarNroOrden(ByVal idOP As Integer) As String
        Dim cmdCampo As SqlCommand = New SqlCommand
        cmdCampo.CommandType = CommandType.Text
        cmdCampo.CommandText = "select case when nroO<100 then '000'+ltrim(str(nroO)) when nroO>=100 and nroO<1000 then '00'+ltrim(str(nroO)) when nroO>=1000 and nroO<10000 then '0'+ltrim(str(nroO)) else ltrim(str(nroO)) end + '-MECH-' + ltrim(str(year(fecOrden))) from TOrdenCompra TOC join TDesOrden TD on TOC.nroOrden=TD.nroOrden where idOP=" & idOP
        cmdCampo.Connection = Cn
        Return cmdCampo.ExecuteScalar
    End Function

    Private Function recuperarUltimoDoc() As Integer
        Dim cmdCampo As SqlCommand = New SqlCommand
        cmdCampo.CommandType = CommandType.Text
        cmdCampo.CommandText = "select MAX(nroDes) from TOrdenDesembolso"
        cmdCampo.Connection = Cn
        Return cmdCampo.ExecuteScalar
    End Function

    Private Function recuperarCount1(ByVal cod As Integer) As Integer
        Dim cmdCampo As SqlCommand = New SqlCommand
        cmdCampo.CommandType = CommandType.Text
        cmdCampo.CommandText = "select count(*) from TPagoDesembolso where idOP=" & cod
        cmdCampo.Connection = Cn
        Return cmdCampo.ExecuteScalar
    End Function

    Private Sub btnElimina_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado("  No existe registro a eliminar...")
            Exit Sub
        End If

        If recuperarUltimoDoc() <> CInt(BindingSource3.Item(BindingSource3.Position)(1)) Then
            MessageBox.Show("No se puede ELIMINAR por no ser la ultima ORDEN DE DESEMBOLSO registrada...", nomNegocio, Nothing, MessageBoxIcon.Error)
            Exit Sub
        End If

        If (recuperarCount1(BindingSource3.Item(BindingSource3.Position)(0)) > 0) Then
            StatusBarClass.messageBarraEstado("  PROCESO DENEGADO, Orden de Desembolso tiene registros en Pago Desembolso...")
            Exit Sub
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de eliminar orden de desembolso Nº " & BindingSource3.Item(BindingSource3.Position)(3), nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        Dim finalMytrans As Boolean = False
        'creando una instancia de transaccion 
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        Try

            StatusBarClass.messageBarraEstado("  ELIMINANDO REGISTROS...")
            'Tabla TDesOrden
            comandoDelete2()
            cmDeleteTable2.Transaction = myTrans
            cmDeleteTable2.ExecuteNonQuery()

            'Tabla TPersDesem
            comandoDelete3()
            cmDeleteTable3.Transaction = myTrans
            cmDeleteTable3.ExecuteNonQuery()

            'Tabla TOrdenDesembolso
            comandoDelete1()
            cmDeleteTable1.Transaction = myTrans
            If cmDeleteTable1.ExecuteNonQuery() < 1 Then
                wait.Close()
                Me.Cursor = Cursors.Default
                myTrans.Rollback()
                MessageBox.Show("No se puede eliminar...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If

            Me.Refresh()

            'confirma la transaccion
            myTrans.Commit()
            StatusBarClass.messageBarraEstado("  REGISTRO FUE ELIMINADO CON EXITO...")
            finalMytrans = True
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            colorearFila()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué eliminado con exito...")

            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            If finalMytrans Then
                MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Information)
                Me.Close()
            Else
                'deshace la transaccion
                myTrans.Rollback()
                MessageBox.Show("Tipo de exception: " & f.Message & Chr(13) & "NO SE ELIMINO EL REGISTRO SELECCIONADO...", nomNegocio, Nothing, MessageBoxIcon.Information)
            End If
        End Try
    End Sub

    Dim cmDeleteTable1 As SqlCommand
    Private Sub comandoDelete1()
        cmDeleteTable1 = New SqlCommand
        cmDeleteTable1.CommandType = CommandType.Text
        cmDeleteTable1.CommandText = "delete from TOrdenDesembolso where idOP=@cod"
        cmDeleteTable1.Connection = Cn
        cmDeleteTable1.Parameters.Add("@cod", SqlDbType.Int, 0).Value = BindingSource3.Item(BindingSource3.Position)(0)
    End Sub

    Dim cmDeleteTable2 As SqlCommand
    Private Sub comandoDelete2()
        cmDeleteTable2 = New SqlCommand
        cmDeleteTable2.CommandType = CommandType.Text
        cmDeleteTable2.CommandText = "delete from TDesOrden where idOP=@cod"
        cmDeleteTable2.Connection = Cn
        cmDeleteTable2.Parameters.Add("@cod", SqlDbType.Int, 0).Value = BindingSource3.Item(BindingSource3.Position)(0)
    End Sub

    Dim cmDeleteTable3 As SqlCommand
    Private Sub comandoDelete3()
        cmDeleteTable3 = New SqlCommand
        cmDeleteTable3.CommandType = CommandType.Text
        cmDeleteTable3.CommandText = "delete from TPersDesem where idOP=@cod"
        cmDeleteTable3.Connection = Cn
        cmDeleteTable3.Parameters.Add("@cod", SqlDbType.Int, 0).Value = BindingSource3.Item(BindingSource3.Position)(0)
    End Sub

    Private Sub btnAperturar1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAperturar1.Click
        vCod2 = "0"  'retornar el NROORDEN

        Dim jala As New jalarOrdenCompra1Form
        jala.ShowDialog()

        If CInt(vCod2) = 0 Then 'se cancelo
            'MsgBox("SE CANCELO")
            Exit Sub
        Else
        End If

        Dim wait As New waitForm
        Me.Cursor = Cursors.WaitCursor
        wait.Show()
        Try
            StatusBarClass.messageBarraEstado("  PROCESANDO DATOS...")

            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.MoveLast()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué guardado con exito...")

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            If CInt(vCod2) > 0 Then
                txtOrden.Text = recuperarNroOrden(BindingSource3.Item(BindingSource3.Position)(0))
            Else
            End If

            colorearFila()

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

    Private Sub btnElimina1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnElimina1.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado("  No existe registro a quitar...")
            Exit Sub
        End If

        If txtOrden.Text.Trim() = "" Then
            MessageBox.Show("No Existe Orden de Compra enlazada a ORDEN DE DESEMBOLSO...", nomNegocio, Nothing, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de quitar orden de compra Nº " & txtOrden.Text.Trim() & " a orden de desembolso Nº " & BindingSource3.Item(BindingSource3.Position)(3), nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        Dim finalMytrans As Boolean = False
        'creando una instancia de transaccion 
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        Try

            StatusBarClass.messageBarraEstado("  ELIMINANDO REGISTROS...")
            Dim idOP As Integer = BindingSource3.Item(BindingSource3.Position)(0)
            'Tabla TDesOrden
            comandoDelete2()
            cmDeleteTable2.Transaction = myTrans
            cmDeleteTable2.ExecuteNonQuery()

            Me.Refresh()

            'confirma la transaccion
            myTrans.Commit()
            StatusBarClass.messageBarraEstado("  REGISTRO FUE ELIMINADO CON EXITO...")
            finalMytrans = True
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.Position = BindingSource3.Find("idOP", idOP)

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué quitado con exito...")

            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            If finalMytrans Then
                MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Information)
                Me.Close()
            Else
                'deshace la transaccion
                myTrans.Rollback()
                MessageBox.Show("Tipo de exception: " & f.Message & Chr(13) & "NO SE ELIMINO EL REGISTRO SELECCIONADO...", nomNegocio, Nothing, MessageBoxIcon.Information)
            End If
        End Try
    End Sub

    Private Sub btnAnula_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAnula.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado("  No existe Orden de Desembolso a ANULAR...")
            Exit Sub
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de ANULAR Orden de Desembolso" & Chr(13) & "Serie: " & BindingSource3.Item(BindingSource3.Position)(2) & "  Nº " & BindingSource3.Item(BindingSource3.Position)(3), nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        Me.Refresh()
        Dim finalMytrans As Boolean = False
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        Try
            StatusBarClass.messageBarraEstado("  ESPERE PROCESANDO INFORMACION....")
            Dim idOP As Integer = BindingSource3.Item(BindingSource3.Position)(0)
            'TOrdenDesembolso
            comandoUpdate13()
            cmUpdateTable13.Transaction = myTrans
            If cmUpdateTable13.ExecuteNonQuery() < 1 Then
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
            finalMytrans = True
            StatusBarClass.messageBarraEstado("  LOS DATOS FUERON ACTUALIZADOS CON EXITO...")
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.Position = BindingSource3.Find("idOP", idOP)

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué guardado con exito...")

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            colorearFila()

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué actualizado con exito...")
            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
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
    End Sub

    Dim cmUpdateTable13 As SqlCommand
    Private Sub comandoUpdate13()
        cmUpdateTable13 = New SqlCommand
        cmUpdateTable13.CommandType = CommandType.Text
        cmUpdateTable13.CommandText = "update TOrdenDesembolso set estado=@est,hist=@hist where idOP=@idOP"
        cmUpdateTable13.Connection = Cn
        cmUpdateTable13.Parameters.Add("@est", SqlDbType.Int, 0).Value = 3 '3 = anulado
        cmUpdateTable13.Parameters.Add("@hist", SqlDbType.VarChar, 200).Value = BindingSource3.Item(BindingSource3.Position)(6) & " ANULO " & Now.Date & " " & vPass & "-" & vSUsuario
        cmUpdateTable13.Parameters.Add("@idOP", SqlDbType.Int, 0).Value = BindingSource3.Item(BindingSource3.Position)(0)
    End Sub

    Private Sub btnImprimir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImprimir.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado("  Proceso Denegado, No existe Orden de Compra...")
            Exit Sub
        End If

        vCodDoc = BindingSource3.Item(BindingSource3.Position)(0)
        vParam1 = txtLetraTotal.Text.Trim()
        vParam2 = txtOrden.Text.Trim()
       
        Dim informe As New ReportViewerOrdenDesembolsoForm
        informe.ShowDialog()
    End Sub

    Private Function ValidarCampos1() As Boolean
        If ValidaFecha(txtFec.Text.Trim()) Then
            StatusBarClass.messageBarraEstado(" Registre FECHA VALIDA...")
            txtFec.Focus()
            Return True
        End If
        'Todo OK RAS
        Return False
    End Function

    Private Sub btnF4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnF4.Click
        If BindingSource3.Position = -1 Then
            StatusBarClass.messageBarraEstado(" No existe Orden de Desembolso a procesar...")
            Exit Sub
        End If

        If cbF4.SelectedIndex = -1 Then
            MessageBox.Show("Seleccione Opción valida...", nomNegocio, Nothing, MessageBoxIcon.Exclamation)
            cbF4.Focus()
            Exit Sub
        End If

        Dim codPersDes As Integer = recuperarCodPersDesem(BindingSource3.Item(BindingSource3.Position)(0), 4) '4=Contabilidad
        If codPersDes > 0 Then 'Existe firma
            If recuperarCodPers(codPersDes) <> vPass Then 'Usurio no es de dirma inicial
                MessageBox.Show("Proceso Denegado, Usuario no es de la Firma Inicial...", nomNegocio, Nothing, MessageBoxIcon.Exclamation)
                Exit Sub
            End If
        End If

        If txtFec.Text.Trim() <> "" Then
            If ValidarCampos1() Then
                Exit Sub
            End If

            If Not ValidaFecha(txtFec.Text.Trim()) Then
                txtFec.Text = CDate(txtFec.Text.Trim())
            End If
        End If

        If cbF4.SelectedIndex = 0 Then 'Aprobado
            If validaCampoVacioMinCaracNoNumer(txtNroCon.Text.Trim(), 3) Then
                MessageBox.Show("Registre Nro de Comprobante...", nomNegocio, Nothing, MessageBoxIcon.Information)
                txtNroCon.Focus()
                Exit Sub
            End If
            If ValidarCampos1() Then
                Exit Sub
            End If
        End If

        Dim resp As String = MessageBox.Show("Esta segúro de seleccionar opción: " & cbF4.Text.Trim() & " para Orden de Desembolso", nomNegocio, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> 6 Then
            Exit Sub
        End If

        Dim comentario As New CometarioForm
        comentario.ShowDialog()

        recuperarUltimoNro(vSerie)
        Dim campo As Integer = CInt(txtNro.Text)

        Dim finalMytrans As Boolean = False
        Dim wait As New waitForm
        Me.Cursor = Cursors.WaitCursor
        wait.Show()
        'estableciendo una transaccion
        Dim myTrans As SqlTransaction = Cn.BeginTransaction()
        Try
            StatusBarClass.messageBarraEstado("  PROCESANDO DATOS...")
            Me.Refresh()
            Dim opcion As Short
            If cbF4.SelectedIndex = 0 Then
                opcion = 1 'Aprobado
            End If
            If cbF4.SelectedIndex = 1 Then
                opcion = 2 'Observado
            End If
            If cbF4.SelectedIndex = 2 Then
                opcion = 3 'denegado
            End If

            If codPersDes = 0 Then 'no existe firma insertar
                'TPersDesem
                comandoInsert2(BindingSource3.Item(BindingSource3.Position)(0), vPass, opcion, 4, vObs, date1.Value.Date)  '4=Contabilidad
                cmInserTable2.Transaction = myTrans
                If cmInserTable2.ExecuteNonQuery() < 1 Then
                    wait.Close()
                    Me.Cursor = Cursors.Default
                    myTrans.Rollback()
                    MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
            Else 'existe firma actualizar
                'TPersDesem
                comandoUpdate3(opcion, vObs, codPersDes)
                cmUpdateTable3.Transaction = myTrans
                If cmUpdateTable3.ExecuteNonQuery() < 1 Then
                    'deshace la transaccion
                    wait.Close()
                    Me.Cursor = Cursors.Default
                    myTrans.Rollback()
                    MessageBox.Show("Ocurrio un error, por lo tanto no se guardo la información procesada...", nomNegocio, Nothing, MessageBoxIcon.Error)
                    Me.Close()
                    Exit Sub
                End If
            End If

            If opcion = 1 Then 'Aprobado x lo tanto cerrar orden
                'TOrdenDesembolso
                comandoUpdate23(2, txtNroCon.Text.Trim(), txtFec.Text.Trim()) '1=Terminado 2=cerrado
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
            End If

            Dim idOP As Integer = BindingSource3.Item(BindingSource3.Position)(0)
            'confirma la transaccion
            myTrans.Commit()    'con exito RAS

            StatusBarClass.messageBarraEstado("  LOS DATOS FUERON GUARDADOS CON EXITO...")
            finalMytrans = True
            vfVan1 = False   'para selePersDesem() se llama dentro de enlazarText()
            vfVan2 = False  'Enlazar Text  TRUE en boton cancelar

            'Actualizando el dataTable
            dsAlmacen.Tables("VOrdenDesembolso").Clear()
            daTabla1.Fill(dsAlmacen, "VOrdenDesembolso")

            recuperarUltimoNro(vSerie)

            'Buscando por nombre de campo y luego pocisionarlo con el indice
            BindingSource3.Position = BindingSource3.Find("idOP", idOP)

            'Clase definida y con miembros shared en la biblioteca ComponentesRAS
            StatusBarClass.messageBarraEstado("  Registro fué procesado con exito...")

            vfVan1 = True
            vfVan2 = True
            enlazarText()

            wait.Close()
            Me.Cursor = Cursors.Default
        Catch f As Exception
            wait.Close()
            Me.Cursor = Cursors.Default
            If finalMytrans Then
                MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            Else
                myTrans.Rollback()
                MessageBox.Show(f.Message & Chr(13) & "NO SE GUARDO EL REGISTRO...PROBLEMAS DE RED...", nomNegocio, Nothing, MessageBoxIcon.Error)
                Me.Close()
                Exit Sub
            End If
        End Try
    End Sub

    Dim cmUpdateTable23 As SqlCommand
    Private Sub comandoUpdate23(ByVal estado As Short, ByVal nroDoc As String, ByVal fecha As String)
        cmUpdateTable23 = New SqlCommand
        cmUpdateTable23.CommandType = CommandType.Text
        cmUpdateTable23.CommandText = "update TOrdenDesembolso set estado=@est,nroConfor=@nroC,fecEnt=@fec where idOP=@nro"
        cmUpdateTable23.Connection = Cn
        cmUpdateTable23.Parameters.Add("@est", SqlDbType.Int, 0).Value = estado
        cmUpdateTable23.Parameters.Add("@nroC", SqlDbType.VarChar, 30).Value = nroDoc
        cmUpdateTable23.Parameters.Add("@fec", SqlDbType.VarChar, 10).Value = fecha
        cmUpdateTable23.Parameters.Add("@nro", SqlDbType.Int, 0).Value = BindingSource3.Item(BindingSource3.Position)(0)
    End Sub
End Class
