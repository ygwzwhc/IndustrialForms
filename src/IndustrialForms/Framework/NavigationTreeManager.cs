using IndustrialForms.Core.Localization;
using IndustrialForms.Core.Theming;

namespace IndustrialForms.Framework;

/// <summary>
/// 左侧导航树管理器：把导航节点与目标窗体类型关联起来，
/// 点击节点时通过事件通知外部打开对应窗体，实现导航与业务的解耦。
/// </summary>
public sealed class NavigationTreeManager
{
    private readonly TreeView _tree;
    private readonly Dictionary<string, Type> _nodeFormMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _nodeTextCn = new(StringComparer.Ordinal);

    public NavigationTreeManager(TreeView tree)
    {
        _tree = tree;
        _tree.HideSelection = false;
        _tree.FullRowSelect = true;
        _tree.ShowLines = false;
        _tree.ShowPlusMinus = false;
        _tree.ShowRootLines = false;
        _tree.ItemHeight = 40;
        _tree.BorderStyle = BorderStyle.None;
        _tree.BackColor = ThemeColors.SidebarBackground;
        _tree.ForeColor = Color.FromArgb(220, 224, 228);
        _tree.Font = ThemeColors.UiFont(10f);

        _tree.AfterSelect += OnAfterSelect;
    }

    /// <summary>导航节点被选中时触发，参数为目标窗体类型。</summary>
    public event Action<Type>? NodeSelected;

    /// <summary>注册一个导航节点。</summary>
    /// <param name="key">唯一标识，用于关联窗体类型。</param>
    /// <param name="text">显示文本（中文基准）。</param>
    /// <param name="formType">目标窗体类型。</param>
    /// <param name="parentKey">父节点标识，为空则挂到根节点下。</param>
    public void RegisterNode(string key, string text, Type formType, string? parentKey = null)
    {
        _nodeFormMap[key] = formType;
        _nodeTextCn[key] = text;

        var parent = string.IsNullOrEmpty(parentKey) ? null : FindNode(_tree.Nodes, parentKey);
        var node = parent is null
            ? _tree.Nodes.Add(key, text)
            : parent.Nodes.Add(key, text);

        node.Tag = key;
    }

    /// <summary>语言切换后刷新所有节点文本。</summary>
    public void RefreshTexts(ILanguageService language)
    {
        foreach (var (key, textCn) in _nodeTextCn)
        {
            var node = FindNode(_tree.Nodes, key);
            if (node is not null)
            {
                node.Text = language.GetText(textCn);
            }
        }
    }

    private static TreeNode? FindNode(TreeNodeCollection nodes, string key)
    {
        foreach (TreeNode node in nodes)
        {
            if (string.Equals((string?)node.Tag, key, StringComparison.Ordinal))
            {
                return node;
            }

            var found = FindNode(node.Nodes, key);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private void OnAfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is string key && _nodeFormMap.TryGetValue(key, out var formType))
        {
            NodeSelected?.Invoke(formType);
        }
    }
}
