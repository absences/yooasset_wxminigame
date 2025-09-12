//// /*
//// Created by Darsan
//// */
//
//using System.Collections;
//using System.Linq;
//using Game;
//using UnityEngine;
//
//public class TutorialManager : Core.TutorialManager
//{
//    public GameBoard GameBoard => LevelManager.Instance.GameBoard;
//
//    private UIManager UIManager => UIManager.Instance;
//
//    public static bool HasShowTutorial
//    {
//        get => PrefManager.GetBool(nameof(HasShowTutorial));
//        set => PrefManager.SetBool(nameof(HasShowTutorial), value);
//    }
//
//    private IEnumerator Start()
//    {
//        if (HasShowTutorial)
//            yield break;
//
//        yield return new WaitForSeconds(0.1f);
//        HighlightUI(null, showTap: false, fill: false);
//        TutorialPanel.Show();
//        yield return new WaitUntilEvent(TutorialPanel, nameof(TutorialPanel.ClickedNext));
//        TutorialPanel.Hide();
//        ClearHighlightUI();
//        yield return null;
//        yield return PlayGameTutorialEnumerator();
//
//        HasShowTutorial = true;
//    }
//
//    private IEnumerator PlayGameTutorialEnumerator()
//    {
//        DeActivateAllActiveButtons();
//        UIManager.Instance.GamePlayPanel.DisableButtons = true;
//        var list = GameBoard.AllShapes.ToList();
//        foreach (var shape in list)
//        {
//            var coordinate = GameBoard.GetCorrectCoordinateForShape(shape);
//            yield return ShapeMoveStep(shape, coordinate);
//        }
//
//        UIManager.Instance.GamePlayPanel.DisableButtons = false;
//        ClearDeActivatedButtons();
//        HideGesture();
//    }
//
//
//    private void ShowGesture(Vector2 from, Vector2 end)
//    {
//        GestureHighlightPanel.GestureDirection = (end - from).normalized;
//        GestureHighlightPanel.TargetPosition = from;
//        GestureHighlightPanel.IsTap = false;
//        GestureHighlightPanel.GestureDistance = (end - from).magnitude;
//        if (!GestureHighlightPanel.Showing)
//            GestureHighlightPanel.Show();
//    }
//
//    private void HideGesture()
//    {
//        if (GestureHighlightPanel.Showing)
//        {
//            GestureHighlightPanel.Hide();
//        }
//    }
//
//    private IEnumerator ShapeMoveStep(Shape shape, Vector2Int target)
//    {
//        GameBoard.DragOnlyShape = shape;
//        GameBoard.TargetCoordinateToDragOnlyShape = target;
//        GameBoard.AddHighlightCoordinates(
//            shape.GetPieceInWorldCoordinates(GameBoard.GetCorrectCoordinateForShape(shape)));
//        var startPoint = shape.BoxGrid.GetRelativePositionForCoordinate(shape.GrapShapeCoordinate + shape.Coordinate);
//        Debug.Log(shape.GrapShapeCoordinate);
//        var endPoint = shape.BoxGrid.GetRelativePositionForCoordinate(shape.GrapShapeCoordinate + target);
//
//
//        ShowGesture(startPoint, endPoint);
//
//        yield return new WaitUntil(() =>
//        {
//            Debug.Log("Coordiante:" + shape.Coordinate + " Target:" + target);
//            return shape.Coordinate == target;
//        });
//    }
//}