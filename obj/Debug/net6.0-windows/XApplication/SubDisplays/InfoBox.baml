   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.InfoBox   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5          � Title$
  Info��5   	   $	��150��6   $	��300��6#   ���#333333   DefaultWindowStyle#��[  5	   	    � WindowStyle$
 None=�5
   	    � WindowStartupLocation$ CenterOwner=�6M   
����d�$��2q�6�   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   5      �� 5      $��* �6   5      .+�5   
   �� -   5         ��Name$ titleBar��5   .   $��0��6   
���   �6B   �� 5   
   $��0��6   $
��Info��6!   $��Center=�6-   $
��Left=�6H   $��10,0,0,0q�6c   
�������5      � 5   
   -   5        	 �Name$	 
grdContent��5      $��1��6   
 � ColumnDefinitions
 5      �� 5       $��* �6#    5"      �� 5#      $	��50* �6    �� 5$      $	��50* �6    .+�5'      �� -   5         ��Name$ lblInfoText��5)      $��WrapWithOverflow=�5'      $
��Text��68   
�������$��0 10 0 5q�5(   +   $��Center=�6=   $��Center=�5)   -   �� 5+      -   5         ��Name$ btnOK��5+   O   $��Right=�6   $��1��61   $��1��6?   $��OK��6^   $��70��6k   $��30��6v   
���   ����#0099ff
�������$��0 0 10 0q�5.        NormalGuiButton#��[ 5/      50      52      53      54      