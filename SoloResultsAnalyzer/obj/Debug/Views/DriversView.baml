   M S B A M L   `   `   `  ����N  JSoloResultsAnalyzer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:SoloResultsAnalyzer.ViewsSoloResultsAnalyzer.Views  5      +    %SoloResultsAnalyzer.Views.DriversView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:SoloResultsAnalyzer.Views     5      .��5	      �   � ColumnDefinitions  5
   
   �� 5      $��* �6   �� 5      $��5* �6    � RowDefinitions 5   
   �� 5      $��* �6   .+�5   
   � $��0��6   $��0��6!   $	��Top=�61   $��Center=�6I    � Orientation$ Vertical=�6f   .+�5      �� $��50��6   $��50��6    $
��Home��6-   $��20q�6U   $��5q�6a   ��6<   �� *Home+6a   �� 5      $��50��6   $��50��6    $
��Save��6-   $��20q�6U   $��5q�6a   ��6<   �� *Save+6a   5      &   System.Windows.Controls.DataGrid  5   
   $��0��6   $��1��6       AutoGenerateColumns .     SelectionMode$ Single=�5         CanUserDeleteRows .     CanUserAddRows .     CanUserResizeRows .   �� AlternationCount$ 2��5      	   AlternatingRowBackground
	 �������5   0   �� 
 �� Path$
 Drivers� �� IsAsync .  �� SelectedItem 5      �� *SelectedDriver+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
First Name��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 2* 6W   1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� *	FirstName+6W     5      $ 	Last Name��6%   $ 2* 6U    68   �� *
LastName+6U   4  .System.Windows.Controls.DataGridCheckBoxColumn  5      $ Ladies��6)   $ * 6V    69   �� *
IsLadies+6V     5      $ Novice��6)   $ * 6V    69   �� *
IsNovice+6V   5!      5"      5#      