   M S B A M L   `   `   `  ���� D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX      GameTimeX.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:GameTimeX     -   ���#333333  � WindowStyle$
  None=� � Title$ 	GameTimeX��$	��800��$
��1200�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � 	Resources  %  !StartStopButtonHoverNotMonitoring g� Duration �� 
TargetName �� TargetProperty  ButtonHoverColor	 �� To ' #StartStopButtonNoHoverNotMonitoring  ButtonDefaultColor " StartStopButtonHoverMonitoring  ButtonHoverMonitoringColor $  StartStopButtonNoHoverMonitoring   ButtonDefaultMonitoringColor��%  &	        7  &	 t     7  &	 �     7  &	 \    7  �� $ 00:00:4D�.���� $ btnStartStopMonitoring��$( "Background.(SolidColorBrush.Color)�8   $ 0:0:0.2D��� $ 00:00:4D�.���� $ btnStartStopMonitoring��$( "Background.(SolidColorBrush.Color)�8   $ 0:0:0.2D��� $ 00:00:4D�.���� $ btnStartStopMonitoring��$( "Background.(SolidColorBrush.Color)�8   $ 0:0:0.2D��� $ 00:00:4D�.���� $ btnStartStopMonitoring��$( "Background.(SolidColorBrush.Color)�8   $ 0:0:0.2D�'  !System.Windows.Shell.WindowChrome	   WindowChrome	   
   GlassFrameThickness$
 0q�   CornerRadius$ 0��   CaptionHeight$ 0Q�.���  � RowDefinitions �� $��30 ��� $��* �.+��� -    ��Name$ titleBar��$��0��
���   ��� $��0��$��	GameTimeX��$��Center=�$
��Left=�$��10,0,0,0q�
��������� $��0�� �� Orientation$ 
Horizontal=�$��Right=�.+��� -    ��Name$ btnMinimize��$
��🗕��$��40��$��30��
������ 
������ 
�������  NoMouseOverButtonStyle#��[ �� -   $ btnClose��$��X��$��40��$��30��
������ 
������ 
�������#��[ � -    �Name$ 
grdContent��$��1�� � ColumnDefinitions �� $	��70* ��� $	��30* � �� $��50 ��� $��* �.+�� $��10 10 10 10q� �� $��30 � �� $	��60* ��� $	��10* ��� $	��10* ��� $	��10* ��� $	��10* �.+��� -    ��Name$ txtSearchBar��$��0��$��0�����#5c5c5c$��30��$��Segoe UI)�$��15(�$��
7, 4, 0, 0q�
�������.���� $��0��$��0�� J� IsHitTestVisible$ False��$��Search for Games here��$��Center=�$
��Left=�$��17,1,0,0q�
�����������  �� 
TargetType" ��.���  �� Property � y� �� Value$ 	Collapsed=� �� Triggers ��  �� Binding �� *Text+ �� ElementName$ txtSearchBar�� �� Value  .a���  � y�$ Visible=��� -   $ btnMode��$��0��$��1��$��30��$��30��
������ 
������ $��20(�
������� 	 MenuBarButton#��[	 .���� -    ��Name$ imgMode�� 
 	ModeTiles#��[
 �� -	   $ btnNew��$��0��$��2��$��30��$��30��
������ 
������ $��0q�$��0q�
�������$��20(�#��[	 $��Stretch=�$��Stretch=�.����   CreateNewImageSource#��[ �� -
   $ 	btnDelete��$��0��$��3��$��30��$��30��
������ 
������ $��20(�
�������#��[	 .����   DeleteImageSource#��[ �� -   $ btnSettings��$��0��$��4��$��30��$��30��
������ 
������ $��20(�
�������#��[	 .����   SettingsImageSource#��[ �� -    ��Name$ 	scrollBar��$��10 0 0 0q�$��1��$��0��$��Hidden=�$
��Auto=�$��Stretch=�.��&   System.Windows.Controls.DataGrid  -     Name$ 
dgProfiles��   SelectionUnit$ FullRow=�    CanUserAddRows$  False��!   SelectionMode$! Single=�"   GridLinesVisibility$" 
Horizontal=�$��1��$��Stretch=�$��0��#   
IsReadOnly$
# True��$   AutoGenerateColumns$$ False��%   RowHeaderWidth$% 0Q����� $ 	scrollBar��& �� Path$& ActualWidth����� *	Profile+ =  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow  DisableSelection*  $System.Windows.Controls.DataGridCell!  System.Windows.SystemColors'   ControlTextBrushKey( �� Color��%  '        '  �     '  �    &	 #    (�� �    *1/SystemColors.InactiveSelectionHighlightBrushKey+)�� "  .���  � �� �#289afc��  � ��
 �������  � ��$
 Bold"���  � ��$ 30����  � ��$ 10 0 0 0q���  � ��$ 0 0 0 2q��� "  .���  � ��$ 50����  � �� �#333333��  � ��
 �������  � ��$ 0q���  � ��$ 30(� ��  �� *State+
 State1.a���  � �� �#333333��  �� *State+
 State2.a���  � �� �#008ae6�� "  .���  � ��$ 0q���   �@ HorizontalGridLinesBrush �#292928��  � �� �#333333�� "  .���  � |�$ False����  � ��
 ���� ��  � ��# � ' ��  � ��
 ���� �� $( #008ae6��)   Columns) 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn*   Header$	* PID��+   Width5	  /System.Windows.Controls.DataGridLengthConverter$+ 0	 ,   
Visibility$, Hidden=�1
  +System.Windows.Controls.DataGridBoundColumn- 
  Binding- �� *PID+  $
* Name��$+ *	 - �� *ProfileName+�� -   $ scrollBarTiles��$��10 0 0 0q�$��1��$��0��$��Hidden=�$
��Auto=�$��Stretch=�.��� -   $ grdGameProfiles��� -   $ grdGameInfo��$��1��$��1��.+��� 
�������$��2 0 0 0q�$��0 0 0 15q�._�� .+��  �� $	��10* ��� $	��60* ��� $	��40* �.+��� $ 
Horizontal=�$��Center=�$��0��.+��� -   . ��Name$. lblGameName��$��CharacterEllipsis=�$	��N/A��$��Center=�
�������$��Center=�$	��300��$��20(�/ � ToolTip/ _� 0 ^� InitialShowDelay$0 0�����#333333$��1q�
����d�
�������.���� -   $��lblToolTipGameName��$	��N/A���� -   $ btnEditProfileName��$��30��$��30��
������ 
������ $��5 0 0 0q�$��0q�
�������$��20(�  EditProfileNameButtonStyle#��[ $��Stretch=�$��Stretch=�.����   EditImageSource#��[ $��0 0 10 0q��� $��1��$ Vertical=�.+��� $��30 10 30 30q�1 J� Effect1 3  -System.Windows.Media.Effects.DropShadowEffect  2   Color$2 Black��3   	Direction$	3 315Q�4   
BlurRadius$4 10Q�5   ShadowDepth$5 5Q�._��� -   $ currProfileImage��$��Uniform=�$
��Auto��  MyImageSource#��[ �� -   $. lblChangeProfileImage��$��Center=�$��	0 -10 0 0q�
�������$��Change Image��$��	Underline{�  ChangeImageStyle#��[ � $��2��$��0 0 0 0q� �� $	��15* ��� $	��15* ��� $	��15* ��� $	��45* � �� $	��50* ��� $	��40* �.+��� $��
Game time:��$��0��$��0��$
��Left=�
�������$��20(�$��Center=�$��30 0 0 0q��� $ 
Horizontal=�$��0��$��1��$
��Left=�.+��� -   $. lblGameTime��$	��N/A��$��0��$��1��$
��Left=�
�������$��20(�$��Center=��� -   $ btnGameTimeInfo��$��25��$��20��$0 0��6 ^� HorizontalOffset$6 20��7 ^� VerticalOffset$	7 -50��
������ 
������ $��10 0 0 0q�$��0q�
�������$��20(�#��[ $��Stretch=�$��Stretch=�/ _� $0 0�����#333333$��1q�
����d�
�������.���� -   $��lblToolTipGameTimeText��	0 Minutes.����   GameInfoSource#��[ $��15��$��15��$��Center=�$��Center=�� -   8 �Name$8 gameTimeInfoButton��9 � 	StaysOpen$9 False��: � PlacementTarget: �� $ btnGameTimeQuestion��.���� 
����d�$��1q�._��� -   $��	popUpText�����#333333
�������$��12(�$��Testtext��$	��200��$	��200���� $��First time played:��$��1��$��0��$
��Left=�
�������$��15(�$��Center=�$��30 0 0 0q��� -   $. lblFirstTimePlayed��$	��N/A��$��1��$��1��$
��Left=�
�������$��15(�$��Center=��� $��Last time played:��$��2��$��0��$
��Left=�
�������$��15(�$��Center=�$��30 0 0 0q��� -   $. lblLastTimePlayed��$	��N/A��$��2��$��1��$
��Left=�
�������$��15(�$��Center=��� $��3��$��0��$��2��$��Center=�.+��� -   $ btnStartStopMonitoring��$��Start Monitoring��$	��200��$��50��
���   �
�������$��20(�  StartStopMonitoringButton#��[ �� -   $. txtGameSession��$��Center=�$��0 10 0 0q�
�������; � Cursor$;  ��$y�Hidden=�Session: 0 minutes