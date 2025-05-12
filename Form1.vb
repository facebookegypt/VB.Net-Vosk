Imports System.Diagnostics
Imports System.IO

Public Class Form1
    Private audioFilePath As String
    Private modelPath As String

    ' Handle "Select Audio File" button click
    Private Sub btnSelectFile_Click(sender As Object, e As EventArgs) Handles btnSelectModel.Click
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "WAV files (*.wav)|*.wav|All files (*.*)|*.*"
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            audioFilePath = openFileDialog.FileName
            lblModelPath.Text = audioFilePath  ' Update label with selected file path
        End If
    End Sub

    ' Handle "Select Model Folder" button click
    Private Sub btnSelectModel_Click(sender As Object, e As EventArgs) Handles btnSelectModel.Click
        Dim folderDialog As New FolderBrowserDialog()
        If folderDialog.ShowDialog() = DialogResult.OK Then
            modelPath = folderDialog.SelectedPath
            lblModelPath.Text = modelPath  ' Update label with selected model path
        End If
    End Sub

    ' Handle "Transcribe" button click
    Private Async Sub btnTranscribe_Click(sender As Object, e As EventArgs) Handles btnTranscribe.Click
        If String.IsNullOrEmpty(audioFilePath) Then
            MessageBox.Show("Please select an audio file first.")
            Return
        End If
        If String.IsNullOrEmpty(modelPath) Then
            MessageBox.Show("Please select the model folder first.")
            Return
        End If

        btnTranscribe.Enabled = False
        txtTranscription.Text = "Transcribing... Please wait."

        Dim outputFile As String = Path.GetTempFileName()
        Dim scriptPath As String = Path.Combine(Application.StartupPath, "transcribe.py")

        ' Set up the process
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = "python"
        startInfo.Arguments = $"""{scriptPath}"" ""{audioFilePath}"" ""{outputFile}"" ""{modelPath}"""
        startInfo.WorkingDirectory = Application.StartupPath
        startInfo.CreateNoWindow = True
        startInfo.UseShellExecute = False

        Try
            Await Task.Run(Sub()
                               Using process As Process = Process.Start(startInfo)
                                   process.WaitForExit()
                                   If process.ExitCode = 0 Then
                                       Dim transcription As String = File.ReadAllText(outputFile)
                                       Me.Invoke(Sub() txtTranscription.Text = transcription)
                                   Else
                                       Me.Invoke(Sub() txtTranscription.Text = "Transcription failed. Check the Python script or audio file.")
                                   End If
                               End Using
                           End Sub)
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}")
        Finally
            If File.Exists(outputFile) Then
                File.Delete(outputFile)
            End If
            btnTranscribe.Enabled = True
        End Try
    End Sub
End Class