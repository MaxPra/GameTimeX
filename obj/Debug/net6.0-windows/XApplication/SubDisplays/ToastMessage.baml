   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.ToastMessage   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5           DefaultWindowStyle#��[  5   	     � Title$
  Info��5	   	   $	��120��6   $	��300��6#   
������  � WindowStyle$
 None=�5
   	    � WindowStartupLocation$ Manual=�6M    � 
ResizeMode$ NoResize=�5        � ShowInTaskbar$ False��66   $|�False��5   	    J� IsHitTestVisible$ False��6    � AllowsTransparency$
 True��5   	    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5   
      Color$ Black��6   	   
BlurRadius$	 50Q�6)   
   	Direction$	
 180Q�69      RenderingBias$ Quality=�6I      ShadowDepth$ 30Q�6a   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      � .+�5   
   �� ���#333333 �� CornerRadius$ 5��6&   
����d�$��2q�6T    5        5      $ Black��6#   $	 10Q�61   $ 5Q�6A      Opacity$	 0.5Q�6Q   ._�5      �  � RowDefinitions 5      �� 5      $��30 �6$   5       �� 5!      $��* �6$   5"      .+�5$      �� -   5         ��Name$ titleBar��5$   9   $��0��6   
���   �$ 5 5 0 0��5%      5&      �� 5'      -   5         ��Name$ Title��5'   �   $��0��6   $
��Info��6)   $��Center=�65   $
��Left=�6P   $��10,0,0,0q�6k   
�������5(      � 5+      -   5         �Name$ 
grdContent��5+   $   $��1��6    � ColumnDefinitions 5,      �� 5-      $��* �6+    5/      �� 50      $	��50* �6(   .+�53      �� -   5        $ Message��55      $��WrapWithOverflow=�53       $
��Text��6@   
�������$��Center=�54   !   $��Center=�6<   $��20(�6Y   55      56      57      58      5:      5<      