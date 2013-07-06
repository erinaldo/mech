﻿Imports System.Globalization.CultureInfo
Imports System
Imports System.Collections

Public Class cConfigFormControls

    Public Sub ConfigGrilla(ByVal grilla As DataGridView)

        'seleción por filas completas
        grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        'seleccion una a la vez
        grilla.MultiSelect = False

        'For index As Integer = 0 To grilla.Rows.Count - 1
        '    grilla.Rows(index).HeaderCell.Value = index + 1
        'Next

        ' grilla.Rows(0).HeaderCell.Value = 1

    End Sub

    ''' <summary>
    ''' Configurar color de controles seleccionados
    ''' </summary>
    ''' <param name="pControl">Nombre de Control Ej. Label</param>
    ''' <param name="contenedor">Nombre de contenedor GroupBox</param>
    ''' <param name="pColor">Color</param>
    ''' <remarks></remarks>
    Public Sub configurarColorControl(ByVal pControl As String, ByVal contenedor As GroupBox, ByVal pColor As Color)

        For index As Integer = 0 To contenedor.Controls.Count - 1


            Dim tipo As String = contenedor.Controls(index).GetType().ToString()
            If contenedor.Controls(index).GetType().ToString() = "System.Windows.Forms." & pControl Then

                contenedor.Controls(index).ForeColor = pColor

            End If
        Next



    End Sub

    ''' <summary>
    ''' Da formato numerico Ej 1,200.00
    ''' </summary>
    ''' <param name="numero"></param>
    ''' <remarks></remarks>
    Public Sub FormatoContabilidad(ByVal numero As TextBox)

        If String.IsNullOrEmpty(numero.Text) = False Then
            Dim valor As Double = CDbl(numero.Text)
            numero.Text = valor.ToString("0,0.00", System.Globalization.CultureInfo.InvariantCulture)
            'Return numero
        End If

    End Sub

End Class
