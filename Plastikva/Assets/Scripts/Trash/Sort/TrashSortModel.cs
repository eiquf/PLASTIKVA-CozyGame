using R3;
using UnityEngine;

public sealed class TrashSortModel
{
    public Observable<Sprite> OnSprite => _currentSprite;
    public Observable<bool> OnCheck => _isCorrect;
    public Observable<bool> OnCompleted => _isCompleted;
    public Observable<int> OnScore => _score;
    public Observable<(int current, int total)> OnProgress => _progress;

    private readonly ReactiveProperty<Sprite> _currentSprite = new(null);
    private readonly ReactiveProperty<bool> _isCorrect = new(false);
    private readonly ReactiveProperty<bool> _isCompleted = new(false);
    private readonly ReactiveProperty<int> _score = new(0);
    private readonly ReactiveProperty<(int, int)> _progress = new((0, 0));

    private TrashData[] _data = System.Array.Empty<TrashData>();
    private int _index;
    private int _total;

    public void SetData(TrashData[] data)
    {
        _data = data ?? System.Array.Empty<TrashData>();
        _total = _data.Length;
        _index = 0;
        _score.Value = 0;
        _isCompleted.Value = false;
        _progress.Value = (_total == 0) ? (0, 0) : (1, _total);
        PushCurrentSprite();
    }

    public void Submit(bool answerYes)
    {
        if (_index < 0 || _index >= _total) return;

        bool correct = (_data[_index].IsRecyclable == answerYes);
        _isCorrect.Value = correct;
        if (correct == true) _score.Value++;

        _index++;
        if (_index >= _total)
        {
            _progress.Value = (_total, _total);
            _currentSprite.Value = null;
            
            _isCompleted.Value = true;
            return;
        }

        _progress.Value = (_index + 1, _total);
        PushCurrentSprite();
    }

    private void PushCurrentSprite()
    {
        _currentSprite.Value = (_index < _total) ? _data[_index].Icon : null;
    }
}
