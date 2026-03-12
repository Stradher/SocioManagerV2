# Socio Manager V2 - Styling Guide

## Overview
This document describes the comprehensive styling system implemented for the Socio Manager V2 application.

## Style Architecture

### Files Structure
```
SocioManagerV2/
├── Styles/
│   ├── Colors.xaml      # Color palette and brushes
│   └── Styles.xaml      # Control styles and templates
└── App.xaml             # Merges all style resources
```

## Color Palette

### Primary Colors
- **Primary**: `#2C3E50` - Dark blue-gray for headers and main elements
- **Primary Dark**: `#1A252F` - Darker variant
- **Primary Light**: `#34495E` - Lighter variant

### Accent Colors
- **Accent**: `#3498DB` - Bright blue for primary actions
- **Accent Hover**: `#2980B9`
- **Accent Pressed**: `#21618C`

### Semantic Colors
- **Success**: `#27AE60` - Green for positive actions (Save, Add)
- **Danger**: `#E74C3C` - Red for destructive actions (Delete)
- **Warning**: `#F39C12` - Orange for caution actions (Cancel)
- **Neutral**: `#95A5A6` - Gray for neutral actions (Close)

### Background Colors
- **Background**: `#ECF0F1` - Light gray application background
- **Surface**: `#FFFFFF` - White for cards and input fields
- **Hover Background**: `#F8F9FA` - Subtle hover effect

### Text Colors
- **Text Primary**: `#2C3E50` - Main text color
- **Text Secondary**: `#7F8C8D` - Secondary/disabled text
- **Text On Primary**: `#FFFFFF` - Text on colored backgrounds

### Border Colors
- **Border**: `#BDC3C7` - Standard borders
- **Border Light**: `#D5DBDB` - Subtle borders
- **Focus Border**: `#3498DB` - Focus indicator

## Available Styles

### Button Styles

#### PrimaryButtonStyle
Use for main actions (Edit, View Details)
```xaml
<Button Content="Edit" Style="{StaticResource PrimaryButtonStyle}"/>
```

#### SuccessButtonStyle
Use for positive actions (Save, Add, Create)
```xaml
<Button Content="💾 Save" Style="{StaticResource SuccessButtonStyle}"/>
```

#### DangerButtonStyle
Use for destructive actions (Delete, Remove)
```xaml
<Button Content="🗑️ Delete" Style="{StaticResource DangerButtonStyle}"/>
```

#### WarningButtonStyle
Use for caution actions (Cancel editing)
```xaml
<Button Content="⚠️ Cancel" Style="{StaticResource WarningButtonStyle}"/>
```

#### NeutralButtonStyle
Use for neutral actions (Close, Back)
```xaml
<Button Content="✖ Close" Style="{StaticResource NeutralButtonStyle}"/>
```

### Input Styles

#### ModernTextBoxStyle
Standard text input with rounded corners and focus effects
```xaml
<TextBox Text="{Binding Name}" Style="{StaticResource ModernTextBoxStyle}"/>
```

#### ModernDatePickerStyle
Styled date picker matching the theme
```xaml
<DatePicker SelectedDate="{Binding Date}" Style="{StaticResource ModernDatePickerStyle}"/>
```

#### ModernCheckBoxStyle
Checkbox with modern styling
```xaml
<CheckBox Content="Active" Style="{StaticResource ModernCheckBoxStyle}"/>
```

### Text Styles

#### LabelTextBlockStyle
Use for field labels
```xaml
<TextBlock Text="Name *" Style="{StaticResource LabelTextBlockStyle}"/>
```

#### TitleTextBlockStyle
Use for main page titles (24px)
```xaml
<TextBlock Text="Dashboard" Style="{StaticResource TitleTextBlockStyle}"/>
```

#### SubtitleTextBlockStyle
Use for section titles (18px)
```xaml
<TextBlock Text="Members Directory" Style="{StaticResource SubtitleTextBlockStyle}"/>
```

### Container Styles

#### CardBorderStyle
Use for card-like containers with shadow
```xaml
<Border Style="{StaticResource CardBorderStyle}">
    <!-- Content -->
</Border>
```

#### ModernWindowStyle
Applied to all windows for consistent background
```xaml
<Window Style="{StaticResource ModernWindowStyle}">
```

### DataGrid Styles

#### ModernDataGridStyle
Complete DataGrid styling with alternating rows
```xaml
<DataGrid Style="{StaticResource ModernDataGridStyle}"
          ColumnHeaderStyle="{StaticResource ModernDataGridColumnHeaderStyle}"
          RowStyle="{StaticResource ModernDataGridRowStyle}"
          CellStyle="{StaticResource ModernDataGridCellStyle}">
```

## Design Features

### Visual Effects
- **Rounded Corners**: 4-8px radius for modern look
- **Drop Shadows**: Subtle shadows on buttons (hover) and cards
- **Hover Effects**: Visual feedback on interactive elements
- **Press Animation**: Buttons scale down slightly when clicked
- **Focus Indicators**: Blue border on focused inputs

### Typography
- **Font Family**: Segoe UI (Windows standard)
- **Font Sizes**:
  - Titles: 24-28px
  - Subtitles: 18px
  - Labels: 13px (SemiBold)
  - Body: 13-14px

### Spacing
- **Padding**: 10-30px depending on context
- **Margins**: 10-20px for component spacing
- **Input Padding**: 10x8px for comfortable click targets

## Usage Examples

### Main Window Header
```xaml
<Border Background="{StaticResource PrimaryBrush}" Padding="30,20">
    <TextBlock Text="Application Title" 
               FontSize="28" 
               FontWeight="Bold" 
               Foreground="{StaticResource TextOnPrimaryBrush}"/>
</Border>
```

### Form Layout
```xaml
<StackPanel>
    <TextBlock Text="Name *" Style="{StaticResource LabelTextBlockStyle}"/>
    <TextBox Text="{Binding Name}" Style="{StaticResource ModernTextBoxStyle}"/>
    
    <TextBlock Text="Email" Style="{StaticResource LabelTextBlockStyle}"/>
    <TextBox Text="{Binding Email}" Style="{StaticResource ModernTextBoxStyle}"/>
</StackPanel>
```

### Button Group
```xaml
<StackPanel Orientation="Horizontal">
    <Button Content="💾 Save" 
            Style="{StaticResource SuccessButtonStyle}"
            Margin="0,0,15,0"/>
    <Button Content="✖ Cancel" 
            Style="{StaticResource NeutralButtonStyle}"/>
</StackPanel>
```

## Customization

### Changing Colors
Edit `Styles/Colors.xaml` to modify the color palette. All colors are defined as resources and brushes for easy customization.

### Extending Styles
Create new styles based on existing ones:
```xaml
<Style x:Key="CustomButtonStyle" TargetType="Button" BasedOn="{StaticResource BaseButtonStyle}">
    <Setter Property="Background" Value="#YourColor"/>
</Style>
```

## Best Practices

1. **Use Semantic Styles**: Choose button styles based on action type
2. **Consistent Spacing**: Use defined margins/padding values
3. **Emoji Icons**: Use emoji for quick, recognizable icons
4. **Accessibility**: Maintain good color contrast ratios
5. **Responsive**: Use Grid and StackPanel for flexible layouts

## Emoji Icons Reference

- ➕ Add/Create
- 💾 Save
- ✏️ Edit
- 🗑️ Delete
- ✖ Close/Cancel
- ⚠️ Warning/Cancel Edit
- 📧 Email
- 📞 Phone
- 🏠 Address

## Notes

- All styles are defined in `Styles/Styles.xaml`
- Colors are centralized in `Styles/Colors.xaml`
- Resources are merged in `App.xaml` for application-wide availability
- Styles support hover, pressed, and disabled states
- Focus indicators meet accessibility guidelines
