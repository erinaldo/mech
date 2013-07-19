﻿Imports System.Data
Imports System.Data.SqlClient
Imports ComponentesSolucion2008


Public Class SeguimientoOrdenDesembolsoForm

#Region "Variables"
    ''' <summary>
    ''' Desembolsos / Solicitante
    ''' </summary>
    ''' <remarks></remarks>
    Dim BindingSource0 As New BindingSource

    ''' <summary>
    ''' Pagos
    ''' </summary>
    ''' <remarks></remarks>
    Dim BindingSource1 As New BindingSource

    ''' <summary>
    ''' Contabilidad
    ''' </summary>
    ''' <remarks></remarks>
    Dim BindingSource2 As New BindingSource

    ''' <summary>
    ''' Aprobaciones
    ''' </summary>
    ''' <remarks></remarks>
    Dim BindingSource3 As New BindingSource

    ''' <summary>
    ''' Obra/Lugar
    ''' </summary>
    ''' <remarks></remarks>
    Dim BindingSource4 As New BindingSource

    ''' <summary>
    ''' Proveedor
    ''' </summary>
    ''' <remarks></remarks>
    Dim BindingSource5 As New BindingSource


    ''' <summary>
    ''' Instancia de objeto para Customizar grilla
    ''' </summary>
    ''' <remarks></remarks>
    Dim oGrilla As New cConfigFormControls
#End Region

#Region "Métodos"


    ''' <summary>
    ''' Metodo que carga los datos iniciales
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DatosIniciales()
        VerificaConexion()
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor
        wait.Show()
        Dim sele As String = "Select idOP,serie,nroDes,nro,fecDes,estado_desembolso,hist,monto,montoDet,montoDif,obra,proveedor,banco,nroCta,nroDet,datoReq,factCheck,bolCheck,guiaCheck,vouCheck,vouDCheck,reciCheck,otroCheck,descOtro,nroConfor,fecEnt,moneda,simbolo,nombre,apellido,ruc,fono,email,codObra,codIde from VOrdenDesembolsoSeguimiento Order By idOp Desc"
        crearDataAdapterTable(daVDetOrden, sele)

        sele = "Select codDesembolso,fecPago,montoPago,tipoP,moneda,simbolo,nroCue,banco,pagoDet,montoD,nroP,clasif from VPagoDesembolsoSeguimiento"
        crearDataAdapterTable(daTabla1, sele)

        sele = "select idOP,fecEnt,nroConfor  from TOrdenDesembolso"
        crearDataAdapterTable(daTabla2, sele)

        sele = "select idOp,nombre,apellido,Area,Estado,ObserDesem,fecFir from VAprobacionesSeguimiento "
        crearDataAdapterTable(daTabla3, sele)

        sele = "Select codigo,nombre from tLugarTrabajo"
        crearDataAdapterTable(daTabla4, sele)

        sele = "Select codIde,razon from TIdentidad where idTipId=2 order by razon asc"
        crearDataAdapterTable(daTabla5, sele)
        'daTabla1.SelectCommand.Parameters.Add("@idDesembolso", SqlDbType.Int).Value = 0

        Try
            crearDSAlmacen()
            daVDetOrden.Fill(dsAlmacen, "VDesembolsoSeguimiento")
            BindingSource0.DataSource = dsAlmacen
            BindingSource0.DataMember = "VDesembolsoSeguimiento"
            dgDesembolso.DataSource = BindingSource0
            BindingNavigator1.BindingSource = BindingSource0

            daTabla1.Fill(dsAlmacen, "VDesembolsoPagos")
            BindingSource1.DataSource = dsAlmacen
            BindingSource1.DataMember = "VDesembolsoPagos"
            dgPagos.DataSource = BindingSource1 ' 

            daTabla2.Fill(dsAlmacen, "VDesembolsoComprobante")
            BindingSource2.DataSource = dsAlmacen
            BindingSource2.DataMember = "VDesembolsoComprobante"
            dgContabilidad.DataSource = BindingSource2

            daTabla3.Fill(dsAlmacen, "VAprobaciones")
            BindingSource3.DataSource = dsAlmacen
            BindingSource3.DataMember = "VAprobaciones"

            daTabla4.Fill(dsAlmacen, "TLugarTrabajo")
            BindingSource4.DataSource = dsAlmacen
            BindingSource4.DataMember = "TLugarTrabajo"
            cbObra.DataSource = BindingSource4
            cbObra.DisplayMember = "nombre"
            cbObra.ValueMember = "codigo"


            daTabla5.Fill(dsAlmacen, "TIdentidad")
            BindingSource5.DataSource = dsAlmacen
            BindingSource5.DataMember = "TIdentidad"
            cbProveedor.DataSource = BindingSource5
            cbProveedor.DisplayMember = "razon"
            cbProveedor.ValueMember = "codIde"

        Catch f As Exception
            MessageBox.Show(f.Message & Chr(13) & "NO SE PUEDE EXTRAER LOS DATOS DE LA BD, LA RED ESTA SATURADA...", nomNegocio, Nothing, MessageBoxIcon.Error)
            Exit Sub
        Finally
            wait.Close()
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>
    ''' Cutomiza la grila de Contabilidad
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ModificandoColumnaDGVConta()
        oGrilla.ConfigGrilla(dgContabilidad)
        dgContabilidad.ReadOnly = True
        dgContabilidad.AllowUserToAddRows = False
        dgContabilidad.AllowUserToDeleteRows = False
        dgContabilidad.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill


        With dgContabilidad
            .Columns(0).Visible = False
            .Columns("fecEnt").HeaderText = "Fecha Registro"
            .Columns("nroConfor").HeaderText = "Nro Documento"

        End With


    End Sub

    ''' <summary>
    ''' Customiza la grilla Pagos
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ModificandoColumnasDGVPagos()
        oGrilla.ConfigGrilla(dgPagos)
        dgPagos.ReadOnly = True
        dgPagos.AllowUserToAddRows = False
        dgPagos.AllowUserToDeleteRows = False

        'dgPagos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Try

            


            With dgPagos
                'codigo Desembolo
                .Columns("codDesembolso").Visible = False
                'fecha de pago
                .Columns("fecPago").HeaderText = "Fecha"
                '.Columns("fecPago").DisplayIndex = 0
                .Columns("fecPago").Width = 70
                'Simbolo Moneda
                .Columns("simbolo").HeaderText = ""
                .Columns("simbolo").DisplayIndex = 2
                .Columns("simbolo").Width = 30

                'monto Pagado 
                .Columns("montoPago").HeaderText = "Monto"
                .Columns("montoPago").DisplayIndex = 3
                .Columns("montoPago").Width = 80
                .Columns("montoPago").DefaultCellStyle.Format = "N2"
                .Columns("montoPago").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                'Medio de PAgo
                .Columns("tipoP").HeaderText = "Medio Pago"
                .Columns("tipoP").DisplayIndex = 5
                .Columns("tipoP").Width = 220
                'Moneda

                ' .Columns("monea").DisplayIndex = 7
                .Columns("moneda").Visible = False
                'Banco 
                .Columns("banco").HeaderText = "Banco"
                .Columns("banco").DisplayIndex = 7
                .Columns("banco").Width = 100
                'Numero de Cuenta usada
                .Columns("nroCue").HeaderText = "N° Cuenta"
                .Columns("nroCue").DisplayIndex = 8
                .Columns("nroCue").Width = 160

                'Descripcion del pago
                .Columns("pagoDet").Visible = False
                '                .Columns("pagoDet").HeaderText = "Descripción"
                '.Columns("pagoDet").DisplayIndex = 6
                '.Columns("pagoDet").Width = 250

                'Monto de detracción
                .Columns("montoD").HeaderText = "Detracción"
                .Columns("montoD").DisplayIndex = 4
                .Columns("montoD").Width = 80
                .Columns("montoD").DefaultCellStyle.Format = "N2"
                .Columns("montoD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight


                'numero Operacion /cheque
                .Columns("nroP").HeaderText = "N°_Op./Cheq."
                .Columns("nroP").Width = 85
                .Columns("nroP").DisplayIndex = 6
                'Clasificación de pagos 
                .Columns("clasif").Visible = False
                '  .Columns("clasif").HeaderText = "Clasificación"
                '.Columns("clasif").Width = 100

            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Customisa la grilla Desembolsos
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ModificandoColumnasDGV()

        oGrilla.ConfigGrilla(dgDesembolso)
        dgDesembolso.ReadOnly = True
        dgDesembolso.AllowUserToAddRows = False
        dgDesembolso.AllowUserToDeleteRows = False



        With dgDesembolso

            .Columns("idOP").Visible = False
            .Columns("serie").HeaderText = "Serie"
            .Columns("serie").DisplayIndex = 0
            .Columns("serie").Width = 40
            'NroDesembolso
            .Columns("nroDes").HeaderText = "Nro"
            .Columns("nroDes").DisplayIndex = 1
            .Columns("nroDes").Width = 40
            'Numero
            .Columns("nro").Visible = False
            'Fecha (Solicitud) Desembolso
            .Columns("fecDes").HeaderText = "Fecha"
            .Columns("fecDes").DisplayIndex = 2
            .Columns("fecDes").Width = 70
            'estado
            .Columns("estado_desembolso").HeaderText = "Estado"
            .Columns("estado_desembolso").DisplayIndex = 3
            .Columns("estado_desembolso").Width = 75
            'Datos Historicos
            .Columns("hist").Visible = False
            'Monto de Desembolso
            .Columns("monto").HeaderText = "Monto"
            .Columns("monto").DisplayIndex = 5
            .Columns("monto").Width = 78
            .Columns("monto").DefaultCellStyle.Format = "N2"
            .Columns("monto").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            '.Columns("montoDet").DefaultCellStyle.Font
            'Monto de Detracción
            .Columns("montoDet").HeaderText = "Detracción"
            .Columns("montoDet").DefaultCellStyle.Format = "N2"
            .Columns("montoDet").Width = 78
            .Columns("montoDet").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            'Monto Diferencia 
            .Columns("montoDif").Visible = False
            'Obra 
            .Columns("obra").HeaderText = "Obra/Lugar"
            .Columns("obra").Width = 270
            'Proveedor
            .Columns("proveedor").HeaderText = "Proveedor"
            .Columns("proveedor").Width = 200
            'Forma de pago negociada
            .Columns("banco").Visible = False
            '.Columns("banco").HeaderText = "Forma_Pago"
            'Nro de cuenta del proveedor
            .Columns("nroCta").Visible = False
            'Nro de cuentra para Detracción del proveedor
            .Columns("nroDet").Visible = False
            'Motivo de Desembolso diferente a Orden de compra
            .Columns("datoReq").Visible = False
            '.Columns("datoReq").HeaderText = "Motivo"
            'check de Factura
            .Columns("factCheck").Visible = False
            'check de Boleta
            .Columns("bolCheck").Visible = False
            'check de Guia de remision
            .Columns("guiaCheck").Visible = False
            'check de voucheer
            .Columns("vouCheck").Visible = False
            'check de voucher de Detraccion
            .Columns("vouDCheck").Visible = False
            ' check de recibo
            .Columns("reciCheck").Visible = False
            ' check de otro tipo de documento
            .Columns("otroCheck").Visible = False
            ' descripcion de otro tipo de documento
            .Columns("descOtro").Visible = False
            ' número de documento de conformidad entregado a contabilidad
            .Columns("nroConfor").Visible = False
            'fecha de entrega de documentos a contabilidad
            .Columns("fecEnt").Visible = False
            'Moneda
            .Columns("moneda").Visible = False
            .Columns("simbolo").HeaderText = ""
            .Columns("simbolo").DisplayIndex = 4
            .Columns("simbolo").Width = 30
            .Columns("simbolo").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            'Nombre de Solicitante
            .Columns("nombre").Visible = False
            .Columns("apellido").Visible = False
            'Ruc del proveedor
            .Columns("ruc").Visible = False
            'Telefono Proveedor
            .Columns("fono").Visible = False
            'Dirección de email
            .Columns("email").Visible = False
        End With
    End Sub

    Private Sub enlazarTextConta()
        If dgContabilidad.Rows.Count = 0 Then
            Exit Sub

        Else


        End If
    End Sub


    ''' <summary>
    ''' Enlaza los datos de la grilla Desembolso con los controles del form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub enlazarText()
        If dgDesembolso.Rows.Count = 0 Then
            Exit Sub

        Else
            'Datos de Generales de Orden desembolso
            txtEstadoDesem.Text = BindingSource0.Item(BindingSource0.Position)(5)

            'pinta TextBox txtEstadoDesem
            Dim aCriterios As String() = {"TERMINADO", "ANULADO"}
            Dim aBackColors As Color() = {Color.Green, Color.Red}
            Dim aForeColors As Color() = {Color.White, Color.White}
            colorTextBox(txtEstadoDesem, aCriterios, aBackColors, aForeColors)


            txtNro.Text = BindingSource0.Item(BindingSource0.Position)(2)
            txtFechaDesem.Text = BindingSource0.Item(BindingSource0.Position)(4)
            txtSolicitante.Text = BindingSource0.Item(BindingSource0.Position)(28) + " " + BindingSource0.Item(BindingSource0.Position)(29).ToString()
            txtMonto.Text = BindingSource0.Item(BindingSource0.Position)(27).ToString + " " + BindingSource0.Item(BindingSource0.Position)(7).ToString
            txtDetraccion.Text = BindingSource0.Item(BindingSource0.Position)(27).ToString + " " + BindingSource0.Item(BindingSource0.Position)(8).ToString

            'txtMonto.Text = BindingSource0.Item(BindingSource0.Position)(7).ToString
            'txtDetraccion.Text = BindingSource0.Item(BindingSource0.Position)(8).ToString


            txtFormaPago.Text = BindingSource0.Item(BindingSource0.Position)(12)

            'Datos específicos de desembolso
            txtMotivoDesem.Text = BindingSource0.Item(BindingSource0.Position)(15)
            txtObra.Text = BindingSource0.Item(BindingSource0.Position)(10)
            txtProveedor.Text = BindingSource0.Item(BindingSource0.Position)(11)
            txtRuc.Text = BindingSource0.Item(BindingSource0.Position)(30)

            txtTelefonoProv.Text = BindingSource0.Item(BindingSource0.Position)(31)
            txtEmailProv.Text = BindingSource0.Item(BindingSource0.Position)(32)
            txtCuentaBco.Text = BindingSource0.Item(BindingSource0.Position)(13)
            txtCuentaDetraccion.Text = BindingSource0.Item(BindingSource0.Position)(14)

            'Factura
            If BindingSource0.Item(BindingSource0.Position)(16) = 1 Then
                chkFactura.Checked = True
            Else
                chkFactura.Checked = False
            End If

            'Boleta
            If BindingSource0.Item(BindingSource0.Position)(17) = 1 Then
                chkBoleta.Checked = True
            Else
                chkBoleta.Checked = False
            End If
            'Guia de Remision
            If BindingSource0.Item(BindingSource0.Position)(18) = 1 Then
                chkGuiaRemision.Checked = True
            Else
                chkGuiaRemision.Checked = False
            End If

            'Voucher
            If BindingSource0.Item(BindingSource0.Position)(19) = 1 Then
                chkVoucher.Checked = True
            Else
                chkVoucher.Checked = False
            End If
            'Voucher Detracción
            If BindingSource0.Item(BindingSource0.Position)(20) = 1 Then
                chkVoucherDetraccion.Checked = True
            Else
                chkVoucherDetraccion.Checked = False
            End If
            'Recibo de Egresos
            If BindingSource0.Item(BindingSource0.Position)(21) = 1 Then
                chkReciboEgreso.Checked = True
            Else
                chkReciboEgreso.Checked = False
            End If
            'Otro 
            If BindingSource0.Item(BindingSource0.Position)(22) = 1 Then
                chkOtros.Checked = True
            Else
                chkOtros.Checked = False
            End If

            '   oGrilla.FormatoContabilidad(txtMonto)
            '  oGrilla.FormatoContabilidad(txtDetraccion)

        End If
    End Sub

    ''' <summary>
    ''' Enlaza los dotos de la grilla Pagos con los controles del form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub enlazarTextPagos()
        If dgPagos.Rows.Count = 0 Then
            txtDescripcionPago.Clear()
            txtClasifiPago.Clear()

            Exit Sub
        Else
            If BindingSource1.Position >= 0 Then
                txtDescripcionPago.Text = BindingSource1.Item(BindingSource1.Position)(8)
                txtClasifiPago.Text = BindingSource1.Item(BindingSource1.Position)(11)
            End If

        End If

        'Dando formato a los números

        ' oGrilla.FormatoContabilidad(txtMontoPago)

        'oGrilla.FormatoContabilidad(txtDetraccionPago)

    End Sub

    ''' <summary>
    ''' Pinta el TextBox Seleccionado con los parametros enviados
    ''' </summary>
    ''' <param name="pTextBox">texbox</param>
    ''' <param name="criterios">criterio a evaluar</param>
    ''' <param name="pBackColor">arreglo de colores la fondo</param>
    ''' <param name="pForeColor">arrelgo de colores para letra</param>
    ''' <remarks></remarks>
    Private Sub colorTextBox(ByVal pTextBox As TextBox, ByVal criterios As String(), ByVal pBackColor As Color(), ByVal pForeColor As Color())

        For i As Integer = 0 To criterios.Length - 1
            If pTextBox.Text = criterios.GetValue(i).ToString Then
                pTextBox.BackColor = pBackColor.GetValue(i)
                pTextBox.ForeColor = pForeColor.GetValue(i)
                Exit For
            Else
                pTextBox.BackColor = Color.White
                pTextBox.ForeColor = Color.Black
            End If
        Next


    End Sub

    Private Sub enlazarTextAprobaciones()
        'Dim nro As Integer = BindingSource0.Item(BindingSource0.Position)(0)


        Dim aCriterios As String() = {"APROBADO", "OBSERVADO", "RECHAZADO"}
        Dim aBackColors As Color() = {Color.Green, Color.Yellow, Color.Red}
        Dim aForeColors As Color() = {Color.White, Color.Red, Color.White}

        'Aprobación Gerencia
        If BindingSource3.Count > 1 Then
            txtEstadoGerencia.Text = BindingSource3.Item(1)(4)
            txtNombreGerente.Text = BindingSource3.Item(1)(1) & " " & BindingSource3.Item(1)(2)
        Else
            txtEstadoGerencia.Text = "PENDIENTE"
            txtNombreGerente.Text = ""
            
        End If
        'Aprobación Tesoreria
        If BindingSource3.Count > 2 Then
            txtEstadoTesoreria.Text = BindingSource3.Item(2)(4)
            txtNombreTesoreria.Text = BindingSource3.Item(2)(1) & " " & BindingSource3.Item(2)(2)

        Else
            txtEstadoTesoreria.Text = "PENDIENTE"
            txtNombreTesoreria.Text = ""
        End If
        'Aprobación de Contabilidad
        If BindingSource3.Count > 3 Then
            txtEstadoContab.Text = BindingSource3.Item(3)(4)
            txtNombreConta.Text = BindingSource3.Item(3)(1) & " " & BindingSource3.Item(3)(2)
        Else
            txtEstadoContab.Text = "PENDIENTE"
            txtNombreConta.Text = ""
        End If

        'Pintando el Texbox
        colorTextBox(txtEstadoGerencia, aCriterios, aBackColors, aForeColors)
        colorTextBox(txtEstadoTesoreria, aCriterios, aBackColors, aForeColors)
        colorTextBox(txtEstadoContab, aCriterios, aBackColors, aForeColors)
    End Sub


    ''' <summary>
    ''' configura los colores de los controles
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub configurarColorControl()
        Me.BackColor = BackColorP

        'Color para los labels del contenedor principal
        For i As Integer = 0 To Me.Controls.Count - 1
            If TypeOf Me.Controls(i) Is Label Then 'LABELS
                Me.Controls(i).ForeColor = ForeColorLabel

            End If

            If TypeOf Me.Controls(i) Is CheckBox Then 'CHECKBOX
                Me.Controls(i).ForeColor = ForeColorLabel

            End If

            If TypeOf Me.Controls(i) Is TextBox Then 'TEXTBOX
                CType(Me.Controls(i), TextBox).ReadOnly = True
            End If

            If TypeOf Me.Controls(i) Is GroupBox Then 'TEXTBOX
                For c As Integer = 0 To Me.Controls(i).Controls.Count - 1
                    If TypeOf Me.Controls(i).Controls(c) Is TextBox Then 'TEXTBOX
                        CType(Me.Controls(i).Controls(c), TextBox).ReadOnly = True
                    End If
                Next
            End If
        Next

        'recorriendo tabs de tabcontrol
        For j As Integer = 0 To TabControl1.TabPages.Count - 1

            For index As Integer = 0 To TabControl1.TabPages(j).Controls.Count - 1
                'TabControl1.TabPages(0).Controls
                If TypeOf TabControl1.TabPages(j).Controls(index) Is GroupBox Then
                    TabControl1.TabPages(j).BackColor = BackColorP
                    oGrilla.configurarColorControl("Label", TabControl1.TabPages(j).Controls(index), ForeColorLabel)
                    oGrilla.configurarColorControl("CheckBox", TabControl1.TabPages(j).Controls(index), ForeColorLabel)

                    For k As Integer = 0 To TabControl1.TabPages(j).Controls(index).Controls.Count - 1
                        If TypeOf TabControl1.TabPages(j).Controls(index).Controls(k) Is TextBox Then
                            CType(TabControl1.TabPages(j).Controls(index).Controls(k), TextBox).ReadOnly = True  ' ForeColorLabel
                        End If
                    Next

                End If
                If TypeOf TabControl1.TabPages(j).Controls(index) Is Label Then
                    TabControl1.TabPages(j).Controls(index).ForeColor = ForeColorLabel
                End If
                If TypeOf TabControl1.TabPages(j).Controls(index) Is TextBox Then
                    CType(TabControl1.TabPages(j).Controls(index), TextBox).ReadOnly = True  ' ForeColorLabel
                End If

            Next

        Next

        'Para el Group Box del form
        oGrilla.configurarColorControl("Label", GroupBox2, ForeColorLabel)

    End Sub


    ''' <summary>
    ''' Pinta la grila de Desembolsos
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ColorearGrilla()

        oGrilla.colorearFilasDGV(dgDesembolso, "estado_desembolso", "TERMINADO", Color.Green, Color.White)
        ' oGrilla.colorearFilasDGV(dgDesembolso, "estado_desembolso", "PENDIENTE", Color.Yellow, Color.Red)

        oGrilla.colorearFilasDGV(dgDesembolso, "estado_desembolso", "ANULADO", Color.Red, Color.White)

    End Sub

    ''' <summary>
    ''' filtra la grilla de ordenes de desembolso
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FiltrarGrillaDesembolso()

        If BindingSource4.Position >= 0 And BindingSource5.Position >= 0 Then



            If chkObras.Checked = False And chkProveedor.Checked = False Then
                '                BindingSource0.Filter = "codObra='" & BindingSource4.Item(BindingSource4.Position)(0) & "' and codIde =" & BindingSource5.Item(BindingSource5.Position)(0)
                BindingSource0.Filter = "codObra='" & cbObra.SelectedValue & "' and codIde =" & cbProveedor.SelectedValue

                Exit Sub
            End If
            If (chkObras.Checked) And (chkProveedor.Checked) = False Then
                'BindingSource0.Filter = "codIde =" & BindingSource5.Item(BindingSource5.Position)(0)
                BindingSource0.Filter = "codIde =" & cbProveedor.SelectedValue
                Exit Sub
            End If

            If (chkObras.Checked = False) And (chkProveedor.Checked) Then

                'BindingSource0.Filter = "codObra='" & BindingSource4.Item(BindingSource4.Position)(0) & "'"
                BindingSource0.Filter = "codObra='" & cbObra.SelectedValue & "'"
                Exit Sub
            End If

            If chkObras.Checked And chkProveedor.Checked Then
                BindingSource0.Filter = ""
                Exit Sub
            End If

        End If
    End Sub

    ''' <summary>
    ''' filtra tomando como criterio el estado de desembolso
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FiltrandoPorEstado()

        'Obtiendo el criterio de filtro
        Dim filtro As String = BindingSource0.Filter
        'Ebaluando el creterio de filtrado
        If cbEstadoDesembolso.Text = "TODOS" Or cbEstadoDesembolso.Text = "" Then
            FiltrarGrillaDesembolso()
            Exit Sub
        End If

        'Filtrando de acuerdo a lo necesitado


        If filtro.Contains("estado_desembolso") = False Then
            If filtro.Length > 0 Then
                BindingSource0.Filter = filtro & " and estado_desembolso='" & cbEstadoDesembolso.Text.Trim() & "'"
            Else
                BindingSource0.Filter = filtro & "estado_desembolso='" & cbEstadoDesembolso.Text.Trim() & "'"
            End If
        Else
            FiltrarGrillaDesembolso()
            filtro = BindingSource0.Filter
            If filtro.Length > 0 Then
                BindingSource0.Filter = filtro & " and estado_desembolso='" & cbEstadoDesembolso.Text.Trim() & "'"
            Else
                BindingSource0.Filter = filtro & " estado_desembolso='" & cbEstadoDesembolso.Text.Trim() & "'"
            End If
        End If

    End Sub

    ''' <summary>
    ''' cambia a texto los numeros
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function cambiarNroTotalLetra() As String
        Dim cALetra As New Num2LetEsp  'clase definida por el usuario
        Dim retorna As String = ""
        If BindingSource0.Item(BindingSource0.Position)(27) = "S/." Then    '30=Nuevos solesl
            'If BindingSource10.Item(BindingSource10.Position)(7) = 30 Then
            cALetra.Moneda = "Nuevos Soles"
        Else    'dolares
            cALetra.Moneda = "Dólares Americanos"
        End If
        'Inicia el Proceso para identificar la cantidad a convertir
        If Val(BindingSource0.Item(BindingSource0.Position)(7)) > 0 Then
            cALetra.Numero = Val(CDbl(BindingSource0.Item(BindingSource0.Position)(7)))
            retorna = "SON: " & cALetra.ALetra.ToUpper()
        End If
        Return retorna
    End Function
#End Region

#Region "Eventos"

#End Region


    Private Sub SeguimientoOrdenDesembolsoForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor

        configurarColorControl()

        DatosIniciales()

        ' dgDesembolso.FirstDisplayedScrollingRowIndex = 0


        'Modifica las columnas de Grilla Desembolso
        ModificandoColumnasDGV()
        ModificandoColumnasDGVPagos()
        ModificandoColumnaDGVConta()

        BindingSource1.Filter = "codDesembolso=" & BindingSource0.Item(BindingSource0.Position)(0)
        BindingSource2.Filter = "idOP=" & BindingSource0.Item(BindingSource0.Position)(0)
        BindingSource3.Filter = "idOP=" & BindingSource0.Item(BindingSource0.Position)(0)



        enlazarText()
        enlazarTextAprobaciones()

        wait.Close()
        Me.Cursor = Cursors.Default
    End Sub


    Private Sub txtObra_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObra.TextChanged

    End Sub


    Private Sub dgDesembolso_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgDesembolso.CellClick, dgDesembolso.CellEnter
        enlazarText()


        'filtrando para que muestre los registros de pagos por orden de desembolso seleccionado
        ' 
        BindingSource1.Filter = "codDesembolso=" & BindingSource0.Item(BindingSource0.Position)(0)
        'filtrando para que muestre los registros de contabilidad por orden de desembolso seleccionado
        BindingSource2.Filter = "idOP=" & BindingSource0.Item(BindingSource0.Position)(0)
        BindingSource3.Filter = "idOP=" & BindingSource0.Item(BindingSource0.Position)(0)

        enlazarTextPagos()

        enlazarTextAprobaciones()
        'ModificandoColumnasDGVPagos()
        'ModificandoColumnaDGVConta()

    End Sub



    Private Sub dgPagos_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgPagos.CellClick

        enlazarTextPagos()

    End Sub

    Private Sub dgContabilidad_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgContabilidad.CellClick
        enlazarTextConta()

    End Sub



    Private Sub SeguimientoOrdenDesembolsoForm_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Leave
        Close()
    End Sub

    Private Sub TabControl1_Selecting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles TabControl1.Selecting

        'Comprabando si la grilla tiene la primera columna no visible
        If dgPagos.Columns("codDesembolso").Visible Then
            dgPagos.Columns("codDesembolso").Visible = False
        End If

        If dgContabilidad.Columns(0).Visible Then
            dgContabilidad.Columns(0).Visible = False
        End If
    End Sub


    Private Sub btnCerrar_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCerrar.Click
        dgContabilidad.Dispose()
        dgDesembolso.Dispose()
        dgPagos.Dispose()
        Me.Close()
    End Sub

    Private Sub txtEstadoContab_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtEstadoContab.TextChanged, txtNombreConta.TextChanged

    End Sub

    Private Sub DataGridView1_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        MessageBox.Show(BindingSource3.Item(1)(3).ToString())

    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SeguimientoOrdenDesembolsoForm_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        ColorearGrilla()
    End Sub

    Private Sub dgDesembolso_RowPostPaint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles dgDesembolso.RowPostPaint
        ColorearGrilla()
    End Sub

    Private Sub chkObras_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkObras.CheckedChanged

        If chkObras.Checked Then
            cbObra.Visible = False
        Else
            cbObra.Visible = True
        End If

        FiltrarGrillaDesembolso()
        FiltrandoPorEstado()
    End Sub

    Private Sub chkProveedor_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkProveedor.CheckedChanged
        If chkProveedor.Checked Then
            cbProveedor.Visible = False
        Else
            cbProveedor.Visible = True
        End If

        FiltrarGrillaDesembolso()
        FiltrandoPorEstado()
    End Sub

    Private Sub cbObra_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbObra.SelectedIndexChanged

        FiltrarGrillaDesembolso()
        FiltrandoPorEstado()
    End Sub

    Private Sub cbProveedor_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbProveedor.SelectedIndexChanged
        FiltrarGrillaDesembolso()
        FiltrandoPorEstado()

    End Sub

    Private Sub cbEstadoDesembolso_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbEstadoDesembolso.SelectedIndexChanged
        FiltrandoPorEstado()
    End Sub

    Private Sub btnImprimir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImprimir.Click
        If BindingSource0.Position = -1 Then
            StatusBarClass.messageBarraEstado("  Proceso Denegado, No existe Orden de Compra...")
            Exit Sub
        End If

        vCodDoc = BindingSource0.Item(BindingSource0.Position)(0)
        vParam1 = cambiarNroTotalLetra()
        vParam2 = txtOrdCompra.Text.Trim()

        Dim informe As New ReportViewerOrdenDesembolsoForm
        informe.ShowDialog()
    End Sub
End Class
