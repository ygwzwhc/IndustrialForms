namespace IndustrialForms.Common;

/// <summary>
/// WinForms 控件扩展方法集合。
/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// 跨线程安全地执行 UI 操作：若当前不在 UI 线程，则投递到 UI 线程执行。
    /// </summary>
    public static void InvokeIfRequired(this Control control, Action action)
    {
        if (control.IsDisposed || control.Disposing)
        {
            return;
        }

        if (control.InvokeRequired)
        {
            try
            {
                control.BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
                // 句柄已释放等场景，忽略。
            }
        }
        else
        {
            action();
        }
    }

    /// <summary>
    /// 广度优先递归查找指定名称的控件（不区分大小写）。
    /// </summary>
    public static IEnumerable<Control> FindControlsByName(this Control root, string name)
    {
        if (root is null || string.IsNullOrEmpty(name))
        {
            yield break;
        }

        var queue = new Queue<Control>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (string.Equals(current.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                yield return current;
            }

            foreach (Control child in current.Controls)
            {
                queue.Enqueue(child);
            }
        }
    }

    /// <summary>
    /// 递归遍历控件树（含自身）。
    /// </summary>
    public static IEnumerable<Control> Descendants(this Control root)
    {
        var queue = new Queue<Control>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (Control child in current.Controls)
            {
                queue.Enqueue(child);
            }
        }
    }
}
