import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Point } from '../model';

@Component({
  selector: 'app-game-field',
  templateUrl: './game-field.component.html',
})
export class GameFieldComponent {

  @Input()
  disabled: boolean;

  @Input()
  fieldState: string = '';

  @Output()
  onMove = new EventEmitter<Point>();

  @Output()
  onRedFlag = new EventEmitter<Point>();

  @Output()
  onQuestionFlag = new EventEmitter<Point>();

  @Output()
  onClearFlag = new EventEmitter<Point>();

  move(x: number, y: number) {
    this.onMove.emit({ x, y });
  }

  toggleFlag(x: number, y: number, event: MouseEvent) {

    event.preventDefault(); // Don't display the user agent's UI

    var cell = this.rows[y][x];
    // Cycle thru the flag states
    if (this.isEmpty(cell)) this.onRedFlag.emit({ x, y });
    if (this.isRedFlag(cell)) this.onQuestionFlag.emit({ x, y });
    if (this.isQuestionFlag(cell)) this.onClearFlag.emit({ x, y });
  }

  get height() { return (this.fieldState || '').split('\r\n').length; }
  get width() {
    var x = (this.fieldState || '').indexOf('\r\n');
    if (x < 0) x = (this.fieldState || '').length;
    return x;
  }

  get rows() { return (this.fieldState || '').split('\r\n').map(row => Array.from(row)); }

  isEmpty(cell: string) { return cell === '.'; }
  isRedFlag(cell: string) { return cell === '#'; }
  isQuestionFlag(cell: string) { return cell === '?'; }
  isMine(cell: string) { return cell === 'X'; }
  isUncovered(cell: string) { return !this.isEmpty(cell) && !this.isRedFlag(cell) && !this.isQuestionFlag(cell) && !this.isMine(cell); }

}
