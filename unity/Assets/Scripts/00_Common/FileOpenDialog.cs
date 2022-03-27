using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Windows.Forms;
using Ookii.Dialogs;
public class FileOpenDialog : MonoBehaviour
{
    public enum Type
    {
        Music,Image
    }
    VistaOpenFileDialog OpenDialog;
    Stream openStream = null;
    // Start is called before the first frame update
    void Start()
    {
        OpenDialog = new VistaOpenFileDialog();
        //OpenDialog.FilterIndex = 3;
        OpenDialog.Title = "���� Ž����";
        
    }
    public string FileOpen(Type type)
    {
        switch (type)
        {
            case Type.Music:
                OpenDialog.Filter = "����� ���� (*.wav, *.mp3) | *.wav; *.mp3;";
                break;
            case Type.Image:
                OpenDialog.Filter = "�̹��� ���� (*.jpg, *.png) | *.jpg; *.png;";
                break;
        }
        
        if (OpenDialog.ShowDialog() == DialogResult.OK)
        {
            if((openStream== OpenDialog.OpenFile()) != null)
            {

                return OpenDialog.FileName;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
