# QuickStart
把常用的程序、命令集成在一起的快捷启动软件。

【开发环境】：

       Windows 10 Professional (Cn) 21H2 + Visusl Studio 2022 Enterprise + .Net FrameWork 4.8

【运行要求】：
 
       需安装 .Net FrameWork 4.8

       Download Url: https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer

【使用说明】：
  
       1.可修改ini配置文件，随时修改可执行程序的启动信息。
       
       2.为避免频繁读取ini文件，程序只会在启动时读取一次。故ini文件修改后，需重新运行本程序。
       
       3.根据ini文件所配置的可视按钮个数，自动顺序显示按钮。每行最多显示 8 个，多的会自动显示在下一行。
       
       4.按钮最多可显示 8 行，共 40 个按钮，多于 40 个的配置会被忽视。

       5.对简体中文、繁体中文、日本语、韩国语、英语等五种语言文字提供支持。
       
       6.为操作方便，提供类似于QQ窗体的屏幕靠边停靠功能。
  
       7.ini文件配置简例：
        #############################################
        #[Button_01] - [Button_40] 最大支持40个按钮，40个以上的忽视
        [Button_XX]
        #按钮显示名称
        Button_Name=
        #按钮是否可视【1：可视；1 以外不可视；默认：空 不显示】
        Button_Visible=
        #按钮是否可用【1：可用；1 以外不可用；默认：空 不可用】
        Button_Enable=
        #按钮背景颜色【设定值为 英文单词】
        Button_BackColor=
        #按钮文本颜色【设定值为 英文单词】
        Button_TextColor=
        #启动程序的完整路径和程序名
        Program_Name=
        #程序启动的类型【exe：exe可执行程序；cmd：cmd命令或者*.cmd、*.bat可执行程序；默认：exe】
        Program_Type=
        #程序的启动参数
        Program_Param=
        #############################################

        [Button_01]
        Button_Name=Eclipse_v4.21
        Button_Visible=1
        Button_Enable=1
        Button_BackColor=GreenYellow
        Button_TextColor=OrangeRed
        Program_Name=D:\Eclipse_4.21\eclipse.exe
        Program_Type=exe
        Program_Param=-clean

      8.可在系统托盘菜单栏，设置开机自动启动。

      9.本程序同时只能启动一个实例。

      10.可设置按钮背景颜色，按钮文本颜色。（可参照 docs/Color.html 的颜色名设置）

      11.默认显示的Reboot，Shutdown，请不要随便点！！！！！！
      
【版本更新】：

      8.v22.06
        新增可设置按钮背景颜色，按钮文本颜色
        新增可设置按钮是否可用

      7.v22.05
        新增开机自启动功能，可在任务栏托盘区【开机启动】菜单设置
        新增多实例运行检查，即同时只能启动一个实例
        移除了前版本中对.Net FrameWork 3.5的支持，现只支持.Net FrameWork 4.8
      
      6.v22.03
        窗体屏幕靠边停靠功能增加开关，设置为关的时候，窗体停靠功能即时失效，无需重启程序
        为兼容新老系统及断网状态的运行需要，同时提供.Net FrameWork 3.5/.Net FrameWork 4.8的编译版
        修改按钮显示方式，由[Button_01]~[Button_40]顺序显示改为按配置文件顺序读取

      5.v22.02
        新增窗体不透明度的菜单设置。可在任务栏托盘区【不透明度】菜单设置
        恢复v21.11以前版本删除的窗体最小化按钮

      4.v22.01
        修复按钮显示多字节文字时乱码问题
        修复带多字节文字路径的程序启动错误问题
        新增简体中文、繁体中文、日本语、韩国语、英语等五种语言文字的支持。可在任务栏托盘区【语言】菜单设置
        新增自己找不到窗体在哪时，窗体再现功能。可在任务栏托盘区【显示】菜单操作

      3.v21.12
        新增窗体靠屏幕上、左、右边停靠（已知双屏停靠Bug：停靠主次屏中部左右边时，窗体频繁左右跳动。暂无解）
        新增退出时消息框确认
        修改最大按钮可支持数，从 32个 变更到 40个
      
      2.v21.11
        新增窗体透明度配置
        #主窗体的不透明度(0 ~ 100；默认100，100为100%不透明)
        MainForm_Opacity=100
      
      1.v21.10
        初版发布
