﻿#pragma checksum "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5B9BE1E967818DBBFC4279B29D646BAEB1CD07DC"
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
    /// Rename
    /// </summary>
    public partial class Rename : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle titleBar;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClose;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdContent;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtProfileName;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRename;
        
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
            System.Uri resourceLocater = new System.Uri("/GameTimeX;component/xapplication/subdisplays/rename.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
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
            
            #line 10 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
            ((GameTimeX.Rename)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
            ((GameTimeX.Rename)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.titleBar = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.btnClose = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
            this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);
            
            #line default
            #line hidden
            
            #line 33 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
            this.btnClose.MouseEnter += new System.Windows.Input.MouseEventHandler(this.btnClose_MouseEnter);
            
            #line default
            #line hidden
            
            #line 34 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
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
            this.btnRename = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\..\..\XApplication\SubDisplays\Rename.xaml"
            this.btnRename.Click += new System.Windows.RoutedEventHandler(this.btnRenameProfile_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

