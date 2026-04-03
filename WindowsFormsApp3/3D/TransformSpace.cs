namespace WindowsFormsApp3
{
    public enum TransformSpace
    {
        World,    // Мировые координаты (по умолчанию)
        Local,    // Локальные (относительно модели)
        Camera,   // Относительно камеры
        Screen    // Экранные (2D delta)
    }
}
