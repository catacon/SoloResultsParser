   M S B A M L   `   `   `  ����N  JSoloResultsAnalyzer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:SoloResultsAnalyzer.ViewsSoloResultsAnalyzer.Views  5      M+clr-namespace:SoloResultsAnalyzer.UtilitiesSoloResultsAnalyzer.Utilities  5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5      /    )SoloResultsAnalyzer.Views.EventImportView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:SoloResultsAnalyzer.Views     5      A	utilities+clr-namespace:SoloResultsAnalyzer.Utilities     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5        � 	Resources  5	      8   2SoloResultsAnalyzer.Utilities.BoolToBrushConverter   DriverExists�   �SoloResultsAnalyzer.Utilities.BoolToValueConverter`1[[System.Windows.Media.SolidColorBrush, PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]]   
FalseValue   	TrueValue��%D   &	          5
   
   $ 	LightPink��6>   $ Transparent��6U   5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $	��15* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��5* �6   .+�5   
   � $��0��6   $��2��6$    � Orientation$ Vertical=�65   $	��Top=�6L   $��Center=�6d   .+�5      �� $��50��6   $��50��6    $
��Home��6-   $��10q�6U   $��5q�6a   ��6<   �� *Home+6a   �� 5      $��50��6   $��50��6    $��Import��6-   $��10q�6Y   $��5q�6e   ��6>   �� *Import+6e   �� 5      $��50��6   $��50��6    $��Cancel��6-   $��10q�6Y   $��5q�6e   ��6>   �� *Cancel+6e   �� 5      $��50��6   $��50��6    $
��Save��6-   $��10q�6U   $��5q�6a   ��6<   �� *Save+6a   5      �� 5   
   $��1��6   $��0��6%    �� Orientation$ 
Horizontal=�62   $
��Left=�6K   .+�5      �� $��Event: ��6   $��Center=�6'   �� 5      $��Center=�6   ��62   �� *Events+ �� SelectedItem 6Q   �� *SelectedEvent+ �� Mode$ TwoWay=�	 �� UpdateSourceTrigger$	 PropertyChanged=���5      �� 5       .R�5!      �� ��5"      `� 5#   "   
 �� StringFormat$"
 Event #{0} - {1:MM/dd} - {2}��6/   .1�5$   &   ��  �� Path$ EventNumber�6.   �� 5%   &   $
 Date�6.   �� 5&   &   $ Location�6.   5'   #   5)      5*      5,      5-      &   System.Windows.Controls.DataGrid  5/   
   $��1��6   $��1��6#      AutoGenerateColumns .     SelectionMode$ Single=�50   /      CanUserDeleteRows .     CanUserAddRows .     CanUserResizeRows .   �� AlternationCount$ 2��52         AlternatingRowBackground
 �������5/   0   �� $ EventResults� �� IsAsync .  53      �� *SelectedResult+.  (System.Windows.Interactivity.Interaction   Triggers 54      /  )System.Windows.Interactivity.EventTrigger  55         	EventName$ CellEditEnding��6!   .  (System.Windows.Interactivity.TriggerBase   Actions. 56      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6,   �� *NameChangedCommand+57         Columns 59      0  *System.Windows.Controls.DataGridTextColumn  5:      ,	  &System.Windows.Controls.DataGridColumn 	  Header$ 
First Name��6%    	  Width5
  /System.Windows.Controls.DataGridLengthConverter$ *
 6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� *DriverInfo.FirstName+$	 PropertyChanged=�   ElementStyle 5;      �� 5<       �� 
TargetType" ��6    .�5=      ��  �� Property � �� �� Value 6;   �� *DriverInfo.DriverExists+  �� 	Converter#  [  5>      5@        5A      $ 	Last Name��6%   $ *
 6�    68   �� *DriverInfo.LastName+$	 PropertyChanged=� 5B      �� 5C      " ��6    .�5D      ��  � �� 6;   �� *DriverInfo.DriverExists+#  [  5E      5G        5H      $ Number��6%   $ *
 65   ! 	  
IsReadOnly! .  5I      `� 5J      $
 {0} {1}��6'   .1�5K      �� $ ClassNumber�6&   �� 5L      $ ClassString�6&   5M      5O        5P      $	 Car��6%   ! . $ 3*
 6\    62   �� *Car+6\     5Q      $ Raw Time��6%   ! . $ *
 6{    67   �� *	RawTime+$
 ##0.000��6{     5R      $ PAX Time��6%   ! . $ *
 6{    67   �� *	PaxTime+$
 ##0.000��6{   4  .System.Windows.Controls.DataGridCheckBoxColumn  5S      $ Ladies��6)   $ *
 6�    69   �� *DriverInfo.IsLadies+$	 PropertyChanged=�6�     5T      $ Novice��6)   $ *
 6�    69   �� *DriverInfo.IsNovice+$	 PropertyChanged=�6�   5V      5Y      5Z      