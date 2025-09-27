import { Directive, ElementRef, Input, OnChanges, SimpleChanges } from '@angular/core';

@Directive({
  selector: '[focusWhen]'
})
export class FocusWhenDirective implements OnChanges
{
  @Input('focusWhen') shouldFocus = false;

  constructor(private el: ElementRef<HTMLInputElement>) { }

  ngOnChanges(changes: SimpleChanges)
  {
    if (changes['shouldFocus']?.currentValue === true)
    {
      setTimeout(() =>
      {
        this.el.nativeElement.focus();
        this.el.nativeElement.setSelectionRange(0, 0);
      });
    }
  }
}
