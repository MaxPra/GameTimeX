   M S B A M L   `   `   `  ���� D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX      GameTimeX.Rename   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:GameTimeX     -     � Title$  Create New Profile��$	��150��$	��300�����#333333 � WindowStyle$
 None=� � WindowStartupLocation$ CenterOwner=�
����d�$��2q�'  !System.Windows.Shell.WindowChrome   WindowChrome      GlassFrameThickness$ 0q�   CornerRadius$ 0��   CaptionHeight$ 0Q�.���  � RowDefinitions �� $��30 ��� $��* �.+��� -    ��Name$ titleBar��$��0��
���   ��� $��0��$��Rename Profile��$��Center=�$
��Left=�$��10,0,0,0q�
��������� $��0��	 �� Orientation$	 
Horizontal=�$��Right=�.+��� -   
 ��Name$
 btnClose��$��X��$��40��$��30��
������ 
������ 
�������   NoMouseOverButtonStyle#��[  � -    �Name$ 
grdContent��$��1�� � ColumnDefinitions �� $	��70* ��� $	��30* � �� $	��50* �.+��� -    ��Name$ txtProfileName��$��0��$��0�����#5c5c5c$��30��$��Segoe UI)�$��15(�$��
7, 4, 0, 0q�
�������$��10, 0, 0, 0q�.���� -   $
 	btnRename��$��0��$��1��$
��save��$��70��$��30��
���   ����#0099ff
�������  NormalGuiButton#��[ 