# IndustrialForms

工业级 WinForms 上位机 UI 框架 · .NET 10

一个面向工业上位机 / HMI 桌面应用场景的 **WinForms UI 框架骨架**。它把一套成熟的桌面端架构（依赖注入、多语言、日志、子窗体管理、导航布局、SQLite 数据持久化）沉淀为可复用的模板，让新项目从“搭架构”中解放出来，专注于业务本身。

> 本项目为框架演示代码，不包含任何具体设备的私有通信协议、寄存器定义或机密业务参数。

---

## 特性

| 能力 | 说明 |
| --- | --- |
| 依赖注入分层 | 基于 `Microsoft.Extensions.DependencyInjection`，基础设施 / 窗体分层装配 |
| 多语言 | 中文基准 + 运行时映射，语言切换事件驱动，全窗体自动刷新 |
| 日志系统 | 分级（Debug/Info/Warn/Error）、调用方溯源、按天滚动落盘、历史回填 |
| 子窗体管理 | 单例复用、嵌入宿主面板，避免重复打开叠加 |
| 导航树布局 | 左侧导航树 + 右侧内容区 + 底部状态栏，上位机经典三段式 |
| 主题令牌 | 颜色与字体集中定义，全站统一切换风格 |
| Toast 提示 | 右下角滑入式通知，渐变、动画、重复过滤 |
| 窗体基类 | 统一提供跨线程调用、生命周期钩子、异步加载信号、资源释放 |
| **SQLite 数据持久化** | 参数 + 通信协议单文件存储，零配置、免安装数据库环境 |

---

## 架构设计

核心思想：**UI 与业务解耦，能力通过基类与基础设施下沉，数据通过 SQLite 落库，窗体只写自身逻辑。**

![IndustrialForms 架构](./docs/images/architecture.png)

从上至下四层：

- **表现层**：主窗体与各类业务子窗体；
- **框架层**：子窗体基类、子窗体管理器、导航树、状态栏等可复用框架能力；
- **基础设施层**：依赖注入、多语言、日志、主题、消息中介等通用服务；
- **数据层**：SQLite 负责参数与通信协议配置的持久化，部署时只需一个 `.db` 文件。

---

## 数据持久化：SQLite

工业上位机中，设备运行参数和通信协议配置需要持久化保存。本框架内置基于 SQLite 的数据层，目标就是**“单文件、零配置、免环境”**：

- **单文件部署**：数据库文件随应用一起放在 `data/industrialforms.db`，直接复制到现场即可运行；
- **首次运行自动建表**：无需手动执行 SQL 脚本；
- **参数仓库 `ParameterRepository`**：通用 Key-Value 参数读写；
- **协议仓库 `ProtocolRepository`**：通信协议配置增删改查，协议具体参数（串口波特率、数据位、从站地址等）以 JSON 保存，新增字段无需改表结构；
- **示例数据**：首次启动自动写入若干示例参数和一条通用串口协议，仪表盘会实时读取并展示，验证数据链路真实可用。

> 提示：实际项目里可替换为真实的参数导入 / 协议配置逻辑，这里仅作能力演示。

---

## 项目结构

```
IndustrialForms/
├── src/IndustrialForms/
│   ├── Program.cs                     # 应用入口
│   ├── DependencyInjection.cs         # 服务装配中心
│   ├── Common/                        # 通用工具
│   │   ├── ControlExtensions.cs       # 跨线程、控件查找等扩展
│   │   └── DataGridViewHelper.cs      # 表格统一样式
│   ├── Core/
│   │   ├── Logging/                   # 日志系统
│   │   │   ├── Logger.cs
│   │   │   ├── LogFileManager.cs
│   │   │   └── LogLevel.cs
│   │   ├── Localization/              # 多语言
│   │   │   ├── ILanguageService.cs
│   │   │   └── LanguageService.cs
│   │   ├── Messaging/                 # 窗体间通信
│   │   │   └── FormMediator.cs
│   │   ├── Storage/                   # SQLite 数据持久化
│   │   │   ├── AppDatabase.cs
│   │   │   ├── ParameterRepository.cs
│   │   │   ├── ProtocolRepository.cs
│   │   │   └── CommunicationProtocol.cs
│   │   └── Theming/                   # 主题与提示
│   │       ├── ThemeColors.cs
│   │       └── ToastNotification.cs
│   ├── Framework/                     # 框架核心
│   │   ├── BaseChildForm.cs           # 子窗体基类
│   │   ├── ChildFormManager.cs        # 子窗体管理
│   │   ├── NavigationTreeManager.cs   # 导航树
│   │   └── StatusStripManager.cs      # 状态栏
│   └── UI/                            # 窗体
│       ├── MainForm.cs                # 主窗体
│       ├── DashboardForm.cs           # 示例：仪表盘（含 SQLite 状态展示）
│       ├── SettingsForm.cs            # 示例：设置
│       ├── LogViewerForm.cs           # 日志查看器
│       └── AboutForm.cs               # 示例：关于
├── docs/images/
│   └── architecture.png               # 架构图
├── LICENSE
└── README.md
```

---

## 快速开始

环境要求：`.NET 10 SDK` + Windows。

```bash
dotnet run --project src/IndustrialForms
```

运行后可见：左侧导航树切换页面，右上角菜单可切换中英文，底部状态栏显示实时时钟，仪表盘的数据存储面板会实时读取 SQLite 中的参数与协议数量。

---

## 扩展方式

1. **新增一个业务窗体**：继承 `BaseChildForm`，实现 `InitializeUi()` 构建界面。
2. **注册到导航**：在 `MainForm.BuildLayout()` 中调用 `_navigation.RegisterNode(...)`。
3. **注册到容器**：在 `DependencyInjection.ConfigureServices()` 中 `AddTransient<TForm>()`。
4. **接入真实业务**：在 `Core` 下新增服务层，通过构造函数注入到窗体。
5. **保存参数/协议**：调用 `ParameterRepository.Set(...)` 或 `ProtocolRepository.Add(...)`，自动落库。

---

## 设计要点

- **窗体单例复用**：`ChildFormManager` 保证同一窗体全局唯一，重复打开只切换焦点。
- **跨线程安全**：`BaseChildForm.InvokeIfRequired` 统一处理后台线程更新 UI。
- **异步加载信号**：`LoadCompletedTask` 让父窗体可等待子窗体加载完成。
- **资源释放**：基类重写 `Dispose`，集中退订事件，避免内存泄漏。
- **日志溯源**：`Logger` 自动记录“类名.方法名”，无需手动传来源。
- **动态多语言文本**：含动态值的 Label 在 `OnLanguageChanged` 中手动重设，避免 `RefreshFormText` 翻译拼接串。
- **零配置数据库**：`AppDatabase` 首次启动自动建表并写入示例数据，无需 DBA 介入。

---

## 联系作者

- 邮箱：[wanghaochenemail@163.com](mailto:wanghaochenemail@163.com)
- GitHub：[ygwzwhc](https://github.com/ygwzwhc)

---

## License

[MIT](./LICENSE)
