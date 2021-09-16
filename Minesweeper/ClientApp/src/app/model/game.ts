import { GameResult } from './game-result';
import { Size } from './size';

export interface Game {
  id: string;
  startTime: string;  // TODO: Use luxon
  field: Size;
  mines: number;
  fieldState: string;
  result: GameResult | null;
};
