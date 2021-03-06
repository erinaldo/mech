﻿Imports System.Data
Imports System.Data.SqlClient
Imports ComponentesSolucion2008
Imports CrystalDecisions.Shared

Public Class GastosPorDiaForm2

    ''' <summary>
    ''' instancia de objeto DataManager
    ''' </summary>
    ''' <remarks></remarks>
    Dim oDataManager As New cDataManager

    ''' <summary>
    ''' Gastos (egresos)
    ''' </summary>
    ''' <remarks></remarks>
    Dim bindingSource0 As New BindingSource

    ''' <summary>
    ''' instancia de objeto ConfigControls
    ''' </summary>
    ''' <remarks></remarks>
    Dim oGrilla As New cConfigFormControls

    'variable temporales para fecha de inicio y fin

    Dim _fechaIni As String
    Dim _fechaFin As String



#Region "métodos"

    ''' <summary>
    ''' Customiza la grilla  Detalle GR
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ModificandoColumnasDGV()
        'oGrilla.ConfigGrilla(dgInsumos)
        dgReporte.ReadOnly = True
        dgReporte.AllowUserToAddRows = False
        dgReporte.AllowUserToDeleteRows = False

        'dgPagos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Try

            'If dgReporte.Columns("banco").Visible = True Then
            '    dgReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            'Else
            '    dgReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            'End If

            'If dgReporte.Columns("nroCue").Visible = True Then
            '    dgReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            'Else
            '    dgReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            'End If

            With dgReporte
                'codigo material
                .Columns("fecPago").HeaderText = "Fecha"
                .Columns("fecPago").Width = 75
                'Material
                '.Columns("serie").HeaderText = ""
                '.Columns("serie").Width = 30
                'Codigo de Serie
                .Columns("nroDes").HeaderText = "N°_Des"
                .Columns("nroDes").Width = 55

                .Columns("nroOperacion").HeaderText = "N°_Op."
                .Columns("nroOperacion").Width = 50

                '.Columns("ruc").Visible = False
                .Columns("ruc").HeaderText = "RUC"
                .Columns("ruc").Width = 75

                .Columns("razon").HeaderText = "Proveedor"
                .Columns("razon").Width = 300

                .Columns("banco").Visible = False
                '.HeaderText = "Banco"

                .Columns("bco").HeaderText = "Banco"
                .Columns("banco").Width = 70

                .Columns("nroCue").Visible = False


                '.Columns("nroCue").Width = 140

                .Columns("simbolo").HeaderText = ""
                .Columns("simbolo").Width = 30

                .Columns("montoPago").Visible = False
                '.HeaderText = "Monto"

                .Columns("montoSoles").HeaderText = "Monto_S/."


                .Columns("montoSoles").Width = 80
                .Columns("montoSoles").DefaultCellStyle.Format = "N2"
                .Columns("montoSoles").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight


                .Columns("montoD").Visible = False
                '.HeaderText = "Detracción"
                .Columns("montoDolares").HeaderText = "Monto_$"

                .Columns("montoDolares").Width = 80
                .Columns("montoDolares").DefaultCellStyle.Format = "N2"
                .Columns("montoDolares").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

                .Columns("codigo").Visible = False

                .Columns("nombre").HeaderText = "Obra/Lugar"
                .Columns("nombre").Width = 300



                .Columns("codBan").Visible = False
                .Columns("codMon").Visible = False
                .Columns("idCue").Visible = False
                .Columns("concepto").HeaderText = "Concepto"
                .Columns("concepto").Width = 350
                .Columns("vanEgreso").Visible = False
                .Columns("tipoClasif").Visible = False
                '.Columns("tipoClasif").HeaderText = "Clasificación"

                '.Columns("tipoClasif").DisplayIndex = 2
                '.Columns("tipoClasif").Width = "100"
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message)

        End Try
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

            If TypeOf Me.Controls(i) Is GroupBox Then 'TEXTBOX
                For c As Integer = 0 To Me.Controls(i).Controls.Count - 1
                    oGrilla.configurarColorControl("Label", Me.Controls(i), ForeColorLabel)
                Next
            End If
        Next

        btnCerrar.ForeColor = ForeColorButtom
        btnMostrar.ForeColor = ForeColorButtom

    End Sub

    ''' <summary>
    ''' Añade criterios a los filtros
    ''' </summary>
    ''' <param name="criterio"></param>
    ''' <param name="filtro"></param>
    ''' <remarks></remarks>
    Private Function AddCriterioFiltro(ByVal criterio As String, ByVal filtro As String) As String
        If filtro.Length > 0 Then
            filtro &= " and " & criterio
        Else
            filtro &= " " & criterio
        End If
        Return filtro
    End Function

    ''' <summary>
    ''' Filtra Desembolso 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub filtrando()
        'If BindingSource4.Position >= 0 And BindingSource5.Position >= 0 Then
        bindingSource0.Filter = ""
        Dim pFiltro As String = bindingSource0.Filter
        Dim pCriterio As String
        If bindingSource0.Position >= 0 Then

            If chkBanco.Checked = False Then
                'Dim vAlmacen As Integer = cbAlmacen.SelectedValue
                'If String.IsNullOrEmpty(cbAlmacen.SelectedValue) Then
                '    cbAlmacen.SelectedIndex = 0
                'End If
                pCriterio = "codBan =" & cbBanco.SelectedValue
                pFiltro = AddCriterioFiltro(pCriterio, pFiltro)
                'ocultando la columna banco

                dgReporte.Columns("banco").Visible = False
            Else
                dgReporte.Columns("banco").Visible = True
            End If

            If chkCuenta.Checked = False Then
                If String.IsNullOrEmpty(cbCuenta.SelectedValue) = False Then
                    pCriterio = "idCue=" & cbCuenta.SelectedValue
                    pFiltro = AddCriterioFiltro(pCriterio, pFiltro)
                End If

                dgReporte.Columns("nroCue").Visible = False
            Else
                dgReporte.Columns("nroCue").Visible = True

            End If

            If chkObra.Checked = False Then
                If String.IsNullOrEmpty(cbObra.SelectedValue) = False Then
                    pCriterio = "codigo ='" & cbObra.SelectedValue & "'"
                    pFiltro = AddCriterioFiltro(pCriterio, pFiltro)
                End If
                dgReporte.Columns("nombre").Visible = False
            Else
                dgReporte.Columns("nombre").Visible = True
            End If

            bindingSource0.Filter = pFiltro

        End If

    End Sub

#End Region

    Private Sub GastosPorDiaForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'creando un form de espera
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor


        Me.AcceptButton = btnMostrar

        configurarColorControl()

        oDataManager.CargarCombo("select codBan,banco from TBanco", CommandType.Text, cbBanco, "codBan", "banco")

        oDataManager.CargarCombo("PA_LugarTrabajo", CommandType.StoredProcedure, cbObra, "codigo", "nombre")

        'cerranod el form wait y reestableciendo cursor
        wait.Close()
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub btnMostrar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMostrar.Click
        'creando un form de espera y estableciendo el cursor en espera
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor


        'SET DATEFORMAT dmy  da formato dd/mm/yyyy para las fechas
        Dim sele As String = "SET DATEFORMAT dmy select fecPago,(bco+' '+simbolo+' '+longitud4) bco,nroOperacion,ruc,razon,simbolo,montoPago,montoD,MontoSoles,MontoDolares,banco,nroCue,(serie +'-'+cast(nroDes as varchar)) nroDes,codBan,codMon,idCue,codigo,nombre,concepto,tipoClasif,vanEgreso from VGastosPorDia2 where vanEgreso=0 and fecPago between '" & dtpInicio.Text & "' and '" & dtpFin.Text & "'"
        oDataManager.CargarGrilla(sele, CommandType.Text, dgReporte, bindingSource0)

        'enlanzando con el binding navigator
        BindingNavigator1.BindingSource = bindingSource0


        filtrando()
        ModificandoColumnasDGV()

        'txtTotalSoles.Text = oGrilla.SumarColumnaGrillaArray(dgReporte, "montoPago", _coluSumSoles, _condSumSoles)
        'txtTotalDolares.Text = oGrilla.SumarColumnaGrillaArray(dgReporte, "montoPago", _coluSumDolares, _condSumDolares)



        'txtTotalSoles.Text = oGrilla.SumarColumnaGrilla(dgReporte, "montoPago", "codMon", "30") ' oTabla.Compute("Sum(montoPago)", "codMon=30").ToString()
        'txtTotalDolares.Text = oGrilla.SumarColumnaGrilla(dgReporte, "montoPago", "codMon", "35") 'oTabla.Compute("Sum(montoPago)", "codMon=35").ToString()

        txtTotalSoles.Text = oGrilla.SumarColumnaGrilla(dgReporte, "montoSoles")
        txtTotalDolares.Text = oGrilla.SumarColumnaGrilla(dgReporte, "montoDolares")



        txtTotalSoles.Text = Format(CDbl(txtTotalSoles.Text), "0,0.00")
        txtTotalDolares.Text = Format(CDbl(txtTotalDolares.Text), "0,0.00")

        'Suma Detracciones
        'txtTotalDetraccion.Text = oGrilla.SumarColumnaGrilla(dgReporte, "montoD", "codMon", "30") ' oTabla.Compute("Sum(montoPago)", "codMon=30").ToString()
        'txtDetraccionDolares.Text = oGrilla.SumarColumnaGrilla(dgReporte, "montoD", "codMon", "35") ' oTabla.Compute("Sum(montoPago)", "codMon=30").ToString()

        'txtTotalDetraccion.Text = Format(CDbl(txtTotalDetraccion.Text), "0,0.00")
        'txtDetraccionDolares.Text = Format(CDbl(txtDetraccionDolares.Text), "0,0.00")

        'asignando valores de fecha
        _fechaIni = dtpInicio.Text
        _fechaFin = dtpFin.Text


        'Cerrando el form de espera y reestableciendo el cursor
        wait.Close()
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub GastosPorDiaForm_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Leave
        Me.Close()

    End Sub

    Private Sub btnCerrar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCerrar.Click
        Me.Close()
    End Sub



    Private Sub cbBanco_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbBanco.SelectedIndexChanged
        Try
            If TypeOf cbBanco.SelectedValue Is String Then
                Dim selec As String = "select idCue,nroCue from TCuentaBan where codBan =" & cbBanco.SelectedValue
                oDataManager.CargarCombo(selec, CommandType.Text, cbCuenta, "idCue", "nroCue")

                If cbCuenta.Items.Count = 1 Then

                    chkCuenta.Checked = False

                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub chkBanco_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBanco.CheckedChanged
        If chkBanco.Checked Then
            cbBanco.Visible = False
            chkCuenta.Visible = False
            lblCta.Visible = False
            chkCuenta.Checked = True
        Else

            cbBanco.Visible = True
            If cbCuenta.Items.Count = 1 Then
                chkCuenta.Checked = False

            End If
            chkCuenta.Visible = True
            lblCta.Visible = True
        End If
    End Sub

    Private Sub chkCuenta_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCuenta.CheckedChanged
        If chkCuenta.Checked Then
            cbCuenta.Visible = False
        Else
            cbCuenta.Visible = True
        End If
    End Sub

    Private Sub dtpFin_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpFin.ValueChanged, dtpInicio.ValueChanged

        If dtpInicio.Value > dtpFin.Value Then


            MessageBox.Show("Fechas Invalidas: desde " & dtpInicio.Text.Trim() & " hasta " & dtpFin.Text.Trim & ". Por favor corrija el rango de fechas: ", nomNegocio, Nothing, MessageBoxIcon.Error)
            dtpInicio.Focus()
        End If

    End Sub

    Private Sub chkObra_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkObra.CheckedChanged
        If chkObra.Checked Then
            cbObra.Visible = False
        Else
            cbObra.Visible = True
        End If
    End Sub

    Private Sub btnImp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImp.Click






        If bindingSource0.Position = -1 Then
            StatusBarClass.messageBarraEstado("  Proceso Denegado, No existe datos...")
            Exit Sub
        End If

        'creando un form de espera y estableciendo el cursor en espera
        Dim wait As New waitForm
        wait.Show()
        Me.Cursor = Cursors.WaitCursor

        'Creando las variables para los parametros
        Dim parameters As New ParameterFields

        Dim pFechaIni As ParameterField = New ParameterField
        Dim pFechaFin As ParameterField = New ParameterField

        '
        Dim valorFechaIni As New ParameterDiscreteValue
        Dim valorFechaFin As New ParameterDiscreteValue


        'Definiendo los nombres
        pFechaIni.Name = "pFechaIni"
        pFechaFin.Name = "pFechaFin"

        'Asignando valores
        valorFechaIni.Value = _fechaIni
        valorFechaFin.Value = _fechaFin

        pFechaIni.CurrentValues.Add(valorFechaIni)
        pFechaFin.CurrentValues.Add(valorFechaFin)

        parameters.Add(pFechaIni)
        parameters.Add(pFechaFin)

        Dim datos As DataSetInformesCr = CargarDatos()

        Dim frm As New ReportViewerGastosDia2(datos)

        'verificando la seleccion de un obra
        If chkObra.Checked = False Then
            frm._selectObra = True
        End If


        frm.CrystalReportViewer1.ParameterFieldInfo = parameters

        'Cerrando el form de espera y reestableciendo el cursor
        wait.Close()
        Me.Cursor = Cursors.Default

        frm.ShowDialog()

    End Sub


    ''' <summary>
    ''' Carga los datos de la grilla a un datatable de un dataset
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CargarDatos() As DataSetInformesCr
        Dim ds As New DataSetInformesCr

        For Each row As DataGridViewRow In dgReporte.Rows

            Dim rowInf As DataSetInformesCr.DatosGastosDia2Row = ds.DatosGastosDia2.NewDatosGastosDia2Row
            rowInf.fecPago = CDate(row.Cells("fecPago").Value)
            rowInf.nroOperacion = CStr(row.Cells("nroOperacion").Value)
            rowInf.ruc = CStr(row.Cells("ruc").Value)
            rowInf.razon = CStr(row.Cells("razon").Value)
            rowInf.simbolo = CStr(row.Cells("simbolo").Value)
            rowInf.montoPago = CDbl(row.Cells("montoPago").Value)
            rowInf.montoD = CDbl(row.Cells("montoD").Value)
            rowInf.nroDes = CStr(row.Cells("nroDes").Value)

            rowInf.montoSoles = CDbl(row.Cells("montoSoles").Value)
            rowInf.montoDolares = CDbl(row.Cells("montoDolares").Value)

            If IsDBNull(row.Cells("concepto").Value) Then
                rowInf.concepto = ""
            Else
                rowInf.concepto = CStr(row.Cells("concepto").Value)
            End If
            'banco
            If IsDBNull(row.Cells("bco").Value) Then
                rowInf.banco = ""
            Else
                rowInf.banco = CStr(row.Cells("bco").Value)
            End If
            'obra
            If IsDBNull(row.Cells("nombre").Value) Then
                rowInf.nombre = ""
            Else
                rowInf.nombre = CStr(row.Cells("nombre").Value)
            End If

           

            If IsDBNull(row.Cells("vanEgreso").Value) Then
                rowInf.vanEgreso = ""
            Else
                rowInf.vanEgreso = CInt(row.Cells("vanEgreso").Value)
            End If

            ds.DatosGastosDia2.AddDatosGastosDia2Row(rowInf)

        Next

        Return ds

    End Function

End Class
