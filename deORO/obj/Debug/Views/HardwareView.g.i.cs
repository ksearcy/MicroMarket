﻿#pragma checksum "..\..\..\Views\HardwareView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1132146C7108B7442B3313CE4945DF9AA429FBBF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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
using deORO.Converters;


namespace deORO.Views {
    
    
    /// <summary>
    /// HardwareView
    /// </summary>
    public partial class HardwareView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BillRecyclerText;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border CoinHopperText;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BillRecyclerButton;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CoinHopperButton;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MDBBillText;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MDBCoinText;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MDBBillReset;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Views\HardwareView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MDBCoinReset;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/deORO;component/views/hardwareview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\HardwareView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\Views\HardwareView.xaml"
            ((deORO.Views.HardwareView)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\Views\HardwareView.xaml"
            ((deORO.Views.HardwareView)(target)).Unloaded += new System.Windows.RoutedEventHandler(this.UserControl_Unloaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BillRecyclerText = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.CoinHopperText = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this.BillRecyclerButton = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.CoinHopperButton = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.MDBBillText = ((System.Windows.Controls.Border)(target));
            return;
            case 7:
            this.MDBCoinText = ((System.Windows.Controls.Border)(target));
            return;
            case 8:
            this.MDBBillReset = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.MDBCoinReset = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

