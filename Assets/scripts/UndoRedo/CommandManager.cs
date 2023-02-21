using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : Singleton<CommandManager>
{
    protected CommandManager() { }

    private List<Command> m_UndoList = new List<Command>();
    private List<Command> m_RedoList = new List<Command>();

    public void Clear()
    {
        m_UndoList.Clear();
        m_RedoList.Clear();
    }

    public void ExecuteCommand(Command command)
    {
        command.Execute();
        m_UndoList.Add(command);
        foreach (UvCommand redo in m_RedoList)
            redo.ReleaseTexture();
        m_RedoList.Clear();
    }

    public void Undo()
    {
        if (CanPerformUndo())
        {
            Command command;
            if (m_UndoList.Count > 1)
            {
                command = m_UndoList[m_UndoList.Count - 2];                
                command.UnExecute();
            }                
            else
            {
                Shortcuts.Instance.DeleteTexture();
            }
            m_RedoList.Insert(0, m_UndoList[m_UndoList.Count - 1]);
            m_UndoList.RemoveAt(m_UndoList.Count - 1);            
        }
    }

    public void Redo()
    {
        if (CanPerformRedo())
        {
            Command command = m_RedoList[0];
            m_RedoList.RemoveAt(0);
            command.Execute();
            m_UndoList.Add(command);
        }
    }

    public bool CanPerformUndo()
    {
        return m_UndoList.Count != 0;
    }

    public bool CanPerformRedo()
    {
        return m_RedoList.Count != 0;
    }
}
