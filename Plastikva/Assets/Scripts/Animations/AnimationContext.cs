using System;

public class AnimationContext<T>
{
    private IAnimation<T> _animationStrategy;

    public void SetAnimationStrategy(IAnimation<T> strategy) => _animationStrategy = strategy;

    public void PlayAnimation(T t, Action action = null) => _animationStrategy?.PlayAnimation(t, action);
}
