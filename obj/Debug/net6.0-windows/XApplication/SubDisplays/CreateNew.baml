   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.CreateNew   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5          � Title$  Create New Profile��5   	   $	��150��6$   $	��300��61   ���#333333 � WindowStyle$
 None=�5	   	    � WindowStartupLocation$ CenterOwner=�6M   
����d�$��2q�6�   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   5      �� 5      $��* �6   5      .+�5   
   �� -   5         ��Name$ titleBar��5   .   $��0��6   
���   �6B   �� 5   
   $��0��6   $��Create new profile��6!   $��Center=�6;   $
��Left=�6V   $��10,0,0,0q�6q   
�������5      �� 5   
   $��0��6   	 �� Orientation$	 
Horizontal=�6"   $��Right=�6;   .+�5      �� -   5        
 ��Name$
 btnClose��5      $��X��6'   $��40��63   $��30��6>   
������ 
������ 
�������   NoMouseOverButtonStyle#��[  5   6   5       5!      � 5$   
   -   5         �Name$ 
grdContent��5$      $��1��6    � ColumnDefinitions 5%      �� 5&      $	��70* �6#   �� 5'      $	��30* �6#    5)      �� 5*      $	��50* �6    6/   �� 5+      $	��50* �6    6/   .+�5.      �� -   5         ��Name$ txtProfileName��5.   3   $��0��6   $��0��6#   ���#5c5c5c$��30��5/      $��Segoe UI)�6'   $��15(�6=   $��
7, 4, 0, 0q�6K   
�������$��10, 0, 0, 0q�6s   .��6�   50      �� 51      $��0��6   $��0��6%    J� IsHitTestVisible$ False��65   $��Profile Name��6N   $��Center=�6b   $
��Left=�6}   $��19,0,0,0q�6�   
���������52      �� 53       �� 
TargetType" ��6   .�54      ��  �� Property � y� �� Value$ 	Collapsed=�67    �� Triggers 55      �� 56       �� Binding 6*   �� *Text+ �� ElementName$ txtProfileName�� �� Value  6_   .a�57   "   ��  � y�$ Visible=�6?   58      5:      5<      �� 5>      -   5        $
 btnCreateNewProfile��5>   3   $��0��6   $��1��6#   $
��save��6P   $��70��6_   $��30��6j   
���   ����#0099ff
�������  NormalGuiButton#��[ 5A       � 	Resources 5C      ��%_   '��       �� 5D      " ��6   .�5E      ��  �@��CornerRadius$ 15��69   5F      5G      5I      �� 5K      -   5        $ 
txtPicPath��5K   3   $��1��6   $��0��6#   ���#5c5c5c$��30��5L      $��Segoe UI)�6'   $��15(�6=   $��
7, 4, 0, 0q�6K   
�������$��10, 0, 0, 0q�6s    �� 
IsReadOnly$
 True��5M      .��6,   5N      �� 5O      -   5        $
 btnShowFileDialog��5O   3   $��1��6   $��1��6#   $��
Choose Pic��6N   $��70��6c   $��30��6n   
���   ����#0099ff
�������#��[ 5R       5T      ��%_   '��       �� 5U      " ��6   .�5V      ��  �@��CornerRadius$ 15��69   5W      5X      5Z      5\      5]      5^      