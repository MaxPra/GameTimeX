﻿#pragma checksum "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E7BA90E664D4D6A5D833175594706F871C5EE3FD"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using GameTimeX;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace GameTimeX {
    
    
    /// <summary>
    /// Properties
    /// </summary>
    public partial class Properties : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle titleBar;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdContent;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtProfileName;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSave;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtPicPath;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnShowFileDialog;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtGameFolderPath;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnShowFileDialogExe;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GameTimeX;component/xapplication/subdisplays/properties.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            ((GameTimeX.Properties)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            ((GameTimeX.Properties)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.titleBar = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);
            
            #line default
            #line hidden
            
            #line 33 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            this.btnClose.MouseEnter += new System.Windows.Input.MouseEventHandler(this.btnClose_MouseEnter);
            
            #line default
            #line hidden
            
            #line 34 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            this.btnClose.MouseLeave += new System.Windows.Input.MouseEventHandler(this.btnClose_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 4:
            this.grdContent = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.txtProfileName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.btnSave = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            this.btnSave.Click += new System.Windows.RoutedEventHandler(this.btnSave_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.txtPicPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.btnShowFileDialog = ((System.Windows.Controls.Button)(target));
            
            #line 85 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            this.btnShowFileDialog.Click += new System.Windows.RoutedEventHandler(this.btnShowFileDialog_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.txtGameFolderPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.btnShowFileDialogExe = ((System.Windows.Controls.Button)(target));
            
            #line 131 "..\..\..\..\..\XApplication\SubDisplays\Properties.xaml"
            this.btnShowFileDialogExe.Click += new System.Windows.RoutedEventHandler(this.btnShowFileDialogExe_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
