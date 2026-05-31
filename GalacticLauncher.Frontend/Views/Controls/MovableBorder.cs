using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace GalacticLauncher.Frontend.Views.Controls;

public class MovableBorder : Border
{
    private bool _isPressed; 
    private Point _positionInBlock;
    private readonly TranslateTransform _transform;

    public MovableBorder()
    {
        _transform = new TranslateTransform();
        RenderTransform = _transform;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        
        _isPressed = true;
        _positionInBlock = e.GetPosition(null); 
        
        _positionInBlock = new Point(
            _positionInBlock.X - _transform.X,
            _positionInBlock.Y - _transform.Y);

        e.Pointer.Capture(this);
        
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _isPressed = false;
        e.Pointer.Capture(null);
        
        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!_isPressed)
            return;
        
        var currentPosition = e.GetPosition(null);

        _transform.X = currentPosition.X - _positionInBlock.X;
        _transform.Y = currentPosition.Y - _positionInBlock.Y;
        
        base.OnPointerMoved(e);
    }
}