using System;
using System.Collections.Generic;

namespace TSS.UniverseLogic
{
    public class Universe
    {
        uint foodForTick;
        int foodPlaceX1, foodPlaceY1, foodPlaceX2, foodPlaceY2;
        uint poisonForTick;
        int poisonPlaceX1, poisonPlaceY1, poisonPlaceX2, poisonPlaceY2;
        UniverseObject[][] universeMatrix;
        List<Cell> cellList = new List<Cell>(0);
        int width, height;
        long ticksCount ;
        int blockCellDesc ;
        int maxCellsCount;
        bool disposed;

        public Universe(int width, int height)
        {
            this.width = width;
            this.height = height;
            universeMatrix = new UniverseObject[width][];
            for (int i = 0; i < width; i++)
            {
                universeMatrix[i] = new UniverseObject[height];
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    AddUniverseObject(i, j, new UniverseObject(), false);

                }
            }
            Initialize();
        }

        //public Universe(UniverseObject[][] universeMatrix, int tick)
        //{
        //    ticksCount = tick;
        //    this.universeMatrix = universeMatrix;
        //    width = universeMatrix.Length;
        //    height = universeMatrix[0].Length;
        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //        {

        //            if (universeMatrix[i][j] == null)
        //                AddUniverseObject(i, j, new UniverseObject(), false);
        //            else
        //            {
        //                universeMatrix[i][j].SetCords(i, j);
        //                universeMatrix[i][j].SetUniverse(this);
        //                if (universeMatrix[i][j] is Cell)
        //                    cellList.Add(universeMatrix[i][j] as Cell);
        //            }

        //        }
        //    }
        //    Initialize();
        //}

        void Initialize()
        {
            disposed = false;
            foodForTick = 0;
            poisonForTick = 0;
            ticksCount = 0;
            blockCellDesc = 0;
            SetMaxCellPersent(1);
            SetFoodPlace(0, 0, width, height);
            SetPoisonPlace(0, 0, width, height);
        }

        public void Dispose()
        {
            disposed = true;
            cellList = null;
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (universeMatrix[i][j] != null)
                    {
                        universeMatrix[i][j].Dispose();
                        universeMatrix[i][j].SetUniverse(null);
                    }
                    universeMatrix[i][j] = null;
                }
            }
            universeMatrix = null;

        }

        public bool IsDisposed()
        {
            return disposed;
        }

        public bool SetFoodPlace(int x1, int y1, int x2, int y2)
        {
            if (ValidateCords2(x1, y1) && ValidateCords2(x2, y2))
                if (x1 <= x2 && y1 <= y2)
                {
                    foodPlaceX1 = x1;
                    foodPlaceY1 = y1;
                    foodPlaceX2 = x2;
                    foodPlaceY2 = y2;
                    return true;
                }
            return false;
        }

        public bool SetPoisonPlace(int x1, int y1, int x2, int y2)
        {
            if (ValidateCords2(x1, y1) && ValidateCords2(x2, y2))
                if (x1 <= x2 && y1 <= y2)
                {
                    poisonPlaceX1 = x1;
                    poisonPlaceY1 = y1;
                    poisonPlaceX2 = x2;
                    poisonPlaceY2 = y2;
                    return true;
                }
            return false;
        }

        public void SetMaxCellPersent(float maxCellsPersentInUniverse)
        {
            maxCellsCount = (int)(maxCellsPersentInUniverse * width * height);
        }

        public void SetMaxCellCount(int maxCellCountInUniverse)
        {
            maxCellsCount = maxCellCountInUniverse;
        }

        //work with object helpers
        //work with object helpers
        //work with object helpers
        bool ValidateCords(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < width && y < height);
        }
        bool ValidateCords2(int x, int y)
        {
            return (x >= 0 && y >= 0 && x <= width && y <= height);
        }


        void SetMatrixElement(int x, int y, UniverseObject universeObject)
        {
            universeMatrix[x][y] = universeObject;
            universeObject.SetCords(x, y);
        }

        public UniverseObject GetObject(int x, int y)
        {
            return universeMatrix[x][y];
        }

        bool AddUniverseObject(int x, int y, UniverseObject universeObject, bool canReSetPrevObject)
        {
            if (universeMatrix[x][y] == null)
            {
                SetMatrixElement(x, y, universeObject);
                universeObject.SetUniverse(this);
                return true;
            }
            else if (canReSetPrevObject || universeMatrix[x][y].isDisposed())
            {
                universeMatrix[x][y].Dispose();
                SetMatrixElement(x, y, universeObject);
                universeObject.SetUniverse(this);
                return true;
            }
            return false;
        }

        void RelocateUniverseObject(int x1, int y1, int x2, int y2)
        {
            SetMatrixElement(x2, y2, universeMatrix[x1][y1]);
            universeMatrix[x1][y1] = new UniverseObject();
        }

        public int GetObjectDescriptor(int x, int y)
        {
            if (ValidateCords(x, y))
                return universeMatrix[x][y].GetDesc();
            return 0;
        }
        //work with object helpers
        //work with object helpers
        //work with object helpers

        public UniverseObject[][] GetUniverseMatrix()
        {
            return (UniverseObject[][])universeMatrix.Clone();
        }

        public int[][] GetAllDescriptors()
        {
            int[][] descriptors = new int[width][];
            for (int i = 0; i < width; i++)
            {
                descriptors[i] = new int[height];
                for (int j = 0; j < height; j++)
                {
                    descriptors[i][j] = universeMatrix[i][j].GetDesc();
                }
            }
            return descriptors;
        }

        public DescAndMoveDir[][] GetAllDescriptorsAndMoveDisp()
        {
            DescAndMoveDir[][] descriptors = new DescAndMoveDir[width][];
            for (int i = 0; i < width; i++)
            {
                descriptors[i] = new DescAndMoveDir[height];
                for (int j = 0; j < height; j++)
                {

                    descriptors[i][j].desc=universeMatrix[i][j].GetDesc();
                    if (universeMatrix[i][j] is Cell)
                        descriptors[i][j].moveDir = (universeMatrix[i][j] as Cell).GetMoveDisperation();
                }
            }
            return descriptors;
        }

        public int GetCellsCount()
        {
            return cellList.Count;
        }


        public void GenerateCells(uint count)
        {
            if (cellList.Count > 0)
            {
                foreach(Cell cell in cellList.ToArray())
                    KillCell(cell.GetX(), cell.GetY());
            }
            List<Cell> bufCellList = new List<Cell>(0);
            for (int i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(width);
                int y = StableRandom.rd.Next(height);
                Cell cell = new Cell(new Genome());
                if (AddUniverseObject(x, y, cell, false))
                    bufCellList.Add(cell);
            }
            cellList = bufCellList;
        }

        void GenerateFood(uint count)
        {
            for (uint i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(foodPlaceX1, foodPlaceX2);
                int y = StableRandom.rd.Next(foodPlaceY1, foodPlaceY2);
                AddUniverseObject(x, y, new Food(FoodType.defaultFood), false);
            }
        }

        void GeneratePoison(uint count)
        {
            for (uint i = 0; i < count; i++)
            {

                int x = StableRandom.rd.Next(poisonPlaceX1, poisonPlaceX2);
                int y = StableRandom.rd.Next(poisonPlaceY1, poisonPlaceY2);
                AddUniverseObject(x, y, new Food(FoodType.poison), false);
            }
        }

        void KillCell(int x, int y)
        {
            if(universeMatrix[x][y] is Cell)
            {
                //if(remove)
                //    cellList.Remove(universeMatrix[x][y] as Cell);
                //universeMatrix[x][y].Dispose();
                AddUniverseObject(x, y, new Food(FoodType.deadCell), true);
                
            }
        }

        void HandleCellMove(Cell cell)
        {
            int x1=cell.GetX(), y1=cell.GetY(), x2, y2;
            MoveDirection md = cell.CalcMoveDirectionAspiration();
            switch (md)
            {
                case MoveDirection.up:
                    x2 = x1; y2 = y1 - 1;
                    break;

                case MoveDirection.down:
                    x2 = x1; y2 = y1 + 1;
                    break;

                case MoveDirection.left:
                    x2 = x1-1; y2 = y1;
                    break;

                case MoveDirection.right:
                    x2 = x1+1; y2 = y1;
                    break;

                default:
                    return;
            }

            if (!ValidateCords(x2, y2))
                return;

            if (universeMatrix[x2][y2].isDisposed())
            {
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else if (universeMatrix[x2][y2] is Food)
            {
                cell.AddEnergy((universeMatrix[x2][y2] as Food).GetEnergyLevel());
                RelocateUniverseObject(x1, y1, x2, y2);
            }
            else if (universeMatrix[x1][y1].GetDesc() == universeMatrix[x2][y2].GetDesc())
            {
                cell.AddEnergy(UniverseConsts.MovesFriendly);
                (universeMatrix[x2][y2] as Cell).AddEnergy(UniverseConsts.MovesFriendly);
            }
            else
            {
                cell.AddEnergy((float)(UniverseConsts.MovesAggression*0.8));
                (universeMatrix[x2][y2] as Cell).AddEnergy(-UniverseConsts.MovesAggression);
                
            }

        }

        void HandleAllCellsMoves()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                HandleCellMove(cellList[i]);
            }
        }

        void CheckAllCells()
        {
            bool notUniverseOverflow = cellList.Count <= maxCellsCount;
            List<Cell> bufCellList = new List<Cell>(0);

            for(int i=0; i<cellList.Count; i++)
            {
                if (cellList[i].isDisposed())
                    continue;         
                if (cellList[i].IncAge() || cellList[i].DecEnergy())
                {
                    KillCell(cellList[i].GetX(), cellList[i].GetY());
                    continue;
                }
                else
                    bufCellList.Add(cellList[i]);


                if (notUniverseOverflow && cellList[i].CalcReproduction() && cellList[i].GetDesc()!=blockCellDesc)
                {
                    List<MoveDirection> md = new List<MoveDirection>(0);
                    int x = cellList[i].GetX();
                    int y = cellList[i].GetY();
                    if (ValidateCords(x,y-1) && universeMatrix[x][y-1].GetDesc()<100)
                        md.Add(MoveDirection.up);
                    if (ValidateCords(x, y + 1) && universeMatrix[x][y + 1].GetDesc() < 100)
                        md.Add(MoveDirection.down);
                    if (ValidateCords(x-1, y ) && universeMatrix[x - 1][y].GetDesc() < 100)
                        md.Add(MoveDirection.left);
                    if (ValidateCords(x+1, y ) && universeMatrix[x + 1][y].GetDesc() < 100)
                        md.Add(MoveDirection.right);


                    MoveDirection choice = MoveDirection.stand;
                    if (md.Count>0)
                        choice = md[StableRandom.rd.Next(md.Count)];

                    if (choice == MoveDirection.stand)
                        continue;

                    Cell newCell = cellList[i].Reproduct();
                    switch (choice)
                    {
                        case MoveDirection.up:
                            AddUniverseObject(x, y - 1, newCell, true);
                            break;

                        case MoveDirection.down:
                            AddUniverseObject(x, y + 1, newCell, true);
                            break;

                        case MoveDirection.left:
                            AddUniverseObject(x - 1, y, newCell, true);
                            break;

                        case MoveDirection.right:
                            AddUniverseObject(x+1, y, newCell, true);
                            break;

                    }
                    bufCellList.Add(newCell);
                }
            }

            List<Cell> bufCellList2;
            if (ticksCount % 11 == 0)
            {
                bufCellList2 = new List<Cell>(0);
                while (bufCellList.Count > 0)
                {
                    int index = StableRandom.rd.Next(bufCellList.Count);
                    bufCellList2.Add(bufCellList[index]);
                    bufCellList.RemoveAt(index);
                }
            }
            else
                bufCellList2 = bufCellList;


            cellList =bufCellList2;
        }

        public void SetFoodForTick(uint value)
        {
            foodForTick = value;
        }

        public void SetPoisonForTick(uint value)
        {
            poisonForTick = value;
        }

        public int GetHeight()
        {
            return height;
        }

        public int GetWidth()
        {
            return width;
        }

        public object[] GetMostFitCell()
        {
            object[] res = new object[2];
            List<int> descriptors = GetCellsDescriptors();
            if (descriptors.Count <= 0)
                return null;


            descriptors.Sort();
            //string ms=@"";
            //for (int i = 1; i < descriptors.Count; i++)
            //{
            //    ms += descriptors[i].ToString() + '\n';
            //}
            //System.Windows.Forms.MessageBox.Show(ms);

            /*
            int numNow = descriptors[0], numHigh=0, repeatsCountNow=1, repeatsCountHigh=0;
            int countDec = descriptors.Count - 1;
            for (int i = 1; i < descriptors.Count; i++)
            {
                if (descriptors[i] == numNow && i != countDec)
                {
                    repeatsCountNow++;
                }
                else
                {
                    if (repeatsCountNow > repeatsCountHigh)
                    {
                        numHigh = numNow;
                        repeatsCountHigh = repeatsCountNow;
                        repeatsCountNow = 1;
                        numNow = descriptors[i];
                    }   

                }
            }*/

            List<int> uniqDescs = new List<int>(0);
            List<int> uniqDescsRepeats = new List<int>(0);
            uniqDescs.Add(descriptors[0]);
            uniqDescsRepeats.Add(1);
            int index = 0;
            for (int i = 1; i < descriptors.Count; i++)
            {
                if (descriptors[i] == uniqDescs[index])
                {
                    uniqDescsRepeats[index]++;
                }
                else
                {
                    uniqDescs.Add(descriptors[i]);
                    uniqDescsRepeats.Add(1);
                    index++;
                }
            }
            int highIndex = 0, highValue = uniqDescsRepeats[0];
            for (int i = 1; i < uniqDescs.Count; i++)
            {
                if (uniqDescsRepeats[i] > highValue)
                {
                    highValue = uniqDescsRepeats[i];
                    highIndex = i;
                }
            }

            if (highValue > UniverseConsts.MaxCellsWithOneType)
                blockCellDesc = uniqDescs[highIndex];
            else
                blockCellDesc = 0;

            res[0] = FindCellByDesc(uniqDescs[highIndex]);
            res[1] = highValue;
            return res;

        }

        Cell FindCellByDesc(int descriptor)
        {
            if (cellList.Count > 0)
            {
                for (int i = 0; i < cellList.Count; i++)
                {
                    if (cellList[i].GetDesc() == descriptor)
                        return cellList[i];
                }
                return cellList[0];
            }
            return null;
        }

        public List<int> GetCellsDescriptors()
        {
            List<int> descriptors = new List<int>(0);
            for(int i = 0; i < cellList.Count; i++)
            {
                descriptors.Add(cellList[i].GetDesc());
            }
            return descriptors;
        }

        public long GetTicksCount()
        {
            return ticksCount;
        }

        public void DoUniverseTick()
		{
            if (!disposed)
            {
                
                HandleAllCellsMoves();
                GenerateFood(foodForTick);
                GeneratePoison(poisonForTick);
                CheckAllCells();

                if (GetCellsCount() == 0)
                    GenerateCells(1);

                ticksCount++;
            }
        }

      
	}
}
