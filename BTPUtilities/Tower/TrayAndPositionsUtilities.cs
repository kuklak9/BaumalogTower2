using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class TrayAndPositionsUtilities
    {

        public static void AddTrayToTrayPositionTable(Dictionary<int, TrayPosition> posin, Tray tray)
        {
            for (int i = tray.Position + 1000 * tray.Column; i < tray.Position + tray.Height + 1000 * tray.Column; i++)
            {
                try
                {
                    posin[i].Tray = tray.ID;
                }
                catch { }
            }

        }


        /// <summary>
        /// sprawdza zwykly switch - 1 w 1 lub półki koło siebie i standardowa zamiana
        /// </summary>
        public static TrayToMove[] GenerateStandardSwitch(Tray tA, Tray tB, Dictionary<int, TrayPosition> posin)
        {
            TrayToMove[] rv = null;
            if (tA == null || tB == null || posin == null)
                return rv;

            TrayToMove ttmA = new TrayToMove();
            TrayToMove ttmB = new TrayToMove();
            ttmA.Tray = tA;
            ttmB.Tray = tB;
            if (tA.Column == tB.Column && tA.Tower == tB.Tower
                && ((tA.Position + tA.Height) == tB.Position || (tB.Position + tB.Height) == tA.Position))
            {
                // A było na Górze
                if (tA.Position > tB.Position)
                {
                    ttmA.NewPosition = tB.Position;
                    ttmB.NewPosition = ttmA.NewPosition + tA.Height;

                    ttmA.NewPositionColumn = tA.Column;
                    ttmB.NewPositionColumn = tB.Column;

                    ttmA.NewPositionTower = tA.Tower;
                    ttmB.NewPositionTower = tB.Tower;
                }
                else
                {
                    ttmB.NewPosition = tA.Position;
                    ttmA.NewPosition = ttmB.NewPosition + tB.Height;

                    ttmA.NewPositionColumn = tA.Column;
                    ttmB.NewPositionColumn = tB.Column;

                    ttmA.NewPositionTower = tA.Tower;
                    ttmB.NewPositionTower = tB.Tower;
                }
            }
            else
            {

                ttmA.NewPosition = ttmB.Tray.Position;
                ttmA.NewPositionColumn = ttmB.Tray.Column;

                ttmB.NewPosition = ttmA.Tray.Position;
                ttmB.NewPositionColumn = ttmA.Tray.Column;

                ttmA.NewPositionTower = tB.Tower;
                ttmB.NewPositionTower = tA.Tower;

            }

            if ((tA.Height == tB.Height) || CheckSwitch(ttmA, ttmB, posin))
            {
                rv = new TrayToMove[2];
                rv[0] = ttmA;
                rv[1] = ttmB;

            }


            return rv;

        }



        public static bool CheckMove(TrayToMove tA, Dictionary<int, TrayPosition> posin, List<int> ignoredTrays)
        {




            if (tA.Tray.Height > 0
                && tA.Tray.ID > 0
                && posin != null)
            {
                if (tA.Tray.Tower != posin.First().Value.Tower)
                    return true;


                int c = 0;
                foreach (var kvp in posin.OrderBy(k => k.Key))
                {
                    if (kvp.Value.Column == tA.NewPositionColumn && kvp.Value.Tower == tA.NewPositionTower && kvp.Value.Position >= tA.NewPosition)
                    {
                        bool posTypeOk = kvp.Value.PositionType == 1 || kvp.Value.Position != tA.NewPosition && kvp.Value.PositionType == 2;
                        bool trayOk = kvp.Value.Tray <= 0 || kvp.Value.Tray == tA.Tray.ID;
                        if (ignoredTrays != null)
                            foreach (var v in ignoredTrays)
                                trayOk = trayOk || kvp.Value.Tray == v;


                        if (posTypeOk && trayOk)
                        {
                            c++;
                            if (c >= tA.Tray.Height)
                                return true;
                        }
                        else
                            return false;
                    }
                }
            }
            return false;

        }

        public static bool CheckSwitch(TrayToMove tA, TrayToMove tB, Dictionary<int, TrayPosition> posin)
        {
            if (tA == null || tB == null || posin == null)
                return false;
            //sprawdz czy A nie nachodzi na B i odwrotnie
            if (tA.NewPositionColumn == tB.NewPositionColumn && tA.NewPositionTower==tB.NewPositionTower)
            {  //w tej samej kolumnie - sprawdź najprostsze nachodzenie *)

                if (tA.NewPosition == tB.NewPosition
                    || tA.NewPosition > tB.NewPosition && tA.NewPosition < tB.NewPosition + tB.Tray.Height
                    || tB.NewPosition > tA.NewPosition && tB.NewPosition < tA.NewPosition + tA.Tray.Height)
                    return false;
            }
            
            return CheckMove(tA, posin, new List<int> { tB.Tray.ID }) && CheckMove(tB, posin, new List<int> { tA.Tray.ID });

        }

        public static int FindGap(Dictionary<int, TrayPosition> posin, int startPos, int column, bool searchDown, List<int> ignoredTrays)
        {

            // find gap
            if (!searchDown)
            {
                foreach (KeyValuePair<int, TrayPosition> kvp in posin.OrderBy(a => a.Value))
                {
                    if (startPos >= kvp.Value.Position && column == kvp.Value.Column)
                    {
                        bool trayOk = kvp.Value.Tray <= 0;
                        if (ignoredTrays != null)
                            foreach (var v in ignoredTrays)
                                trayOk = trayOk || kvp.Value.Tray == v;

                        if (kvp.Value.PositionType == 1 && trayOk)
                        {
                            return kvp.Value.Position;
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, TrayPosition> kvp in posin.OrderByDescending(a => a.Value))
                {
                    if (startPos <= kvp.Value.Position && column == kvp.Value.Column)
                    {
                        bool trayOk = kvp.Value.Tray <= 0;
                        if (ignoredTrays != null)
                            foreach (var v in ignoredTrays)
                                trayOk = trayOk || kvp.Value.Tray == v;

                        if (kvp.Value.PositionType == 1 && trayOk)
                        {
                            return kvp.Value.Position;
                        }

                    }

                }

            }
            return 0;
        }



        public static int[] GetGap(Dictionary<int, TrayPosition> posin, int startPos, int column, bool searchDown, List<int> ignoredTrays)
        {
            int[] rv = new int[2];

            int newStart = FindGap(posin, startPos, column, searchDown, ignoredTrays);
            if (newStart == 0)
                return rv;

            rv[0] = newStart;
            if (searchDown)
                rv[1] = GetMaxGapSizeGoDown(posin, newStart, column);
            else
                rv[1] = GetMaxGapSize(posin, newStart, column);

        
            return rv;
        }




        public static int GetMaxGapSizeGoDown(Dictionary<int, TrayPosition> posin, int startPos, int column)
        {
            int rv = 0;

            foreach (KeyValuePair<int, TrayPosition> kvp in posin.OrderByDescending(a => a.Value))
            {
                if (startPos <= kvp.Value.Position && column == kvp.Value.Column)
                {
                    if ((kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2) && kvp.Value.Tray <= 0)
                    {
                        rv++;

                    }
                    else
                    {
                        break;
                    }
                }

            }

            if (rv != 0)
            {
                foreach (KeyValuePair<int, TrayPosition> kvp in posin.OrderBy(a => a.Value))
                {
                    if (startPos >= kvp.Value.Position && column == kvp.Value.Column)
                    {
                        if ((kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2 && rv != 0) && kvp.Value.Tray <= 0)
                        {
                            rv++;

                        }
                        else if (rv == 0 && kvp.Value.PositionType == 2 && kvp.Value.Tray <= 0)
                        {
                            // do nothing
                        }
                        else
                            break;
                    }

                }


            }

            return rv;
        }

        public static int GetMaxGapSize(Dictionary<int, TrayPosition> posin, int startPos, int column)
        {
            int rv = 0;

            foreach (KeyValuePair<int, TrayPosition> kvp in posin.OrderBy(a => a.Value))
            {
                if (startPos >= kvp.Value.Position && column == kvp.Value.Column)
                {
                    if ((kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2 && rv != 0) && kvp.Value.Tray <= 0)
                    {
                        rv++;

                    }
                    else
                    {
                        break;
                    }


                }

            }

            return rv;
        }


        public static int[] GetAvaiablePositionsKeysForTray(Dictionary<int, TrayPosition> posin, Tray tray)
        {
            List<int> rv = new List<int>();
            try
            {

                Dictionary<int, int> pos_start = new Dictionary<int, int>();
                Dictionary<int, TrayPosition> positions = new Dictionary<int, TrayPosition>(posin);
                foreach (KeyValuePair<int, TrayPosition> kvp in positions.OrderBy(a => a.Value))
                {
                    int position_type = kvp.Value.PositionType;

                    if ((kvp.Value.Tray <= 0 || kvp.Value.Tray == tray.ID) && (position_type == 1 || (position_type == 2)))
                    {
                        if (position_type == 1)
                        {
                            pos_start.Add(kvp.Key, 0);
                        }

                        foreach (int ind in pos_start.Keys.ToList())
                        {
                            pos_start[ind]++;
                        }
                    }
                    else
                    {
                        foreach (int ind in pos_start.Keys)
                        {
                            if (pos_start[ind] >= tray.Height)
                                rv.Add(ind);
                        }

                        pos_start.Clear();
                    }

                }
            }
            catch { }

            try
            {
                return rv.ToArray();
            }
            catch
            {
                return null;
            }
        }

        //public static TrayToMove GetMoveForReorganisation(Dictionary<int, TrayPosition> posin)
        //{
        //    return null;
        //}

        /// <summary>
        /// Liczy ilosc przerw...
        /// </summary>
        /// <param name="posin"></param>
        /// <returns></returns>
        public static int GetCountForReorganisation(Dictionary<int, TrayPosition> posin)//!!!
        {
            int count = 0;
            int count_memo = 0;
            int actualColumn = 0;
            int actualColumn_memo = 0;
            int rv = 0;
            // Dictionary<int, Tray> traysCfg = new Dictionary<int, Tray>(trays);
            Dictionary<int, TrayPosition> positions = new Dictionary<int, TrayPosition>(posin);
            foreach (KeyValuePair<int, TrayPosition> kvp in positions.OrderBy(a => a.Value))// (DataRow dr in TowerTable.Rows)
            {

                if ((kvp.Value.Tray <= 0
                    && (kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2 && count > 0))
                    && (kvp.Value.Column == actualColumn || actualColumn == 0))
                {
                    actualColumn = kvp.Value.Column;
                    count++;
                }
                else // dziura sie skonczyla
                {
                    if (count > 0)
                    {
                        if (kvp.Value.Column == actualColumn_memo)
                        {
                            rv += count_memo;
                        }
                        actualColumn_memo = actualColumn;
                        count_memo = count;
                    }

                    count = 0;

                    actualColumn = 0;
                }
            }

            return rv;
        }

        /// <summary>
        /// PObiera polke z gory i wsadza w wolne miejsce
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private static TrayToMove GetShelfForRemoveEmptySpace_GetTop(Dictionary<int, TrayPosition> posin, Dictionary<int, Tray> trays)
        {
            int newposition = 0;
            int count = 0;
            int actualColumn = 0;
            TrayToMove rv = null;

            Dictionary<int, Tray> traysCfg = new Dictionary<int, Tray>(trays);
            Dictionary<int, TrayPosition> positions = new Dictionary<int, TrayPosition>(posin);
            foreach (KeyValuePair<int, TrayPosition> kvp in positions.OrderBy(a => a.Value))// (DataRow dr in TowerTable.Rows)
            {

                if ((kvp.Value.Tray <= 0
                    && (kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2 && count > 0))
                    && (kvp.Value.Column == actualColumn || actualColumn == 0))
                {

                    if (count == 0)
                    {
                        actualColumn = kvp.Value.Column;
                        newposition = kvp.Value.Position;
                    }

                    count++;
                }
                else // dziura sie skonczyla
                {
                    if (count > 0)
                    {
                        for (int j = count; j > 0; j--)
                        {
                            // rv = FindTopShelfWithHeight(trays, newposition, j, actualColumn);
                            foreach (KeyValuePair<int, Tray> kvpt in traysCfg.OrderByDescending(x => x.Value.Position))
                            {
                                if (kvpt.Value.Height == j && kvpt.Value.Position > newposition && kvpt.Value.Column == actualColumn)
                                {
                                    rv = new TrayToMove();
                                    rv.Tray = kvpt.Value.Copy();
                                    rv.NewPosition = newposition;
                                    rv.NewPositionColumn = actualColumn;
                                    break;

                                }
                            }
                            if (rv != null)
                                break;
                        }

                    }
                    if (rv != null)
                    {
                        rv.NewPosition = newposition;
                        rv.NewPositionColumn = actualColumn;
                        break;
                    }
                    count = 0;
                    newposition = 0;
                    actualColumn = 0;
                }
            }
            if (rv != null && (rv.NewPosition < 1 || rv.NewPositionColumn < 1))
                rv = null;

            return rv;
        }


        //private static TrayToMove FindTopShelfWithHeight(Dictionary<int, Tray> trays, int newposition, int count, int column)
        //{

        //    TrayToMove rv = null;
        //    Tray rvpom = null;
        //    // proba znalezienia takiej samej polki
        //    Dictionary<int, Tray> traysCfg = new Dictionary<int, Tray>(trays);
        //    foreach (KeyValuePair<int, Tray> kvp in traysCfg.OrderByDescending(x => x.Value.Position))
        //    {
        //        int trayposition = kvp.Value.Position;

        //        // jesli wielkosc polki sie zgadza i pozycja jest wyzsza to
        //        if (kvp.Value.Height == count && trayposition > newposition && kvp.Value.Column == column)
        //        {

        //            if (rvpom == null)
        //            {
        //                rvpom = kvp.Value.Copy();
        //                break;
        //                // break...bo podobno sortowane...
        //            }
        //            else
        //            {
        //                if (rvpom.Position < kvp.Value.Position)
        //                {
        //                    rvpom = kvp.Value.Copy();
        //                }
        //            }
        //        }
        //    }

        //    if (rvpom != null)
        //    {
        //        rv = new TrayToMove();
        //        rv.Tray = rvpom;
        //    }
        //    return rv;
        //}


        private static TrayToMove GetShelfForRemoveEmptySpace_MoveDown(Dictionary<int, TrayPosition> posin, Dictionary<int, Tray> trays)
        {
            int newposition = 0;
            int count = 0;
            int actualColumn = 0;

            TrayToMove rv = null;

            Dictionary<int, TrayPosition> positions = new Dictionary<int, TrayPosition>(posin);
            Dictionary<int, Tray> trayCfg = new Dictionary<int, Tray>(trays);
            foreach (KeyValuePair<int, TrayPosition> kvp in positions.OrderBy(a => a.Value))
            {
                if ((kvp.Value.Tray <= 0
                    && (kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2 && count > 0))
                    && (kvp.Value.Column == actualColumn || actualColumn == 0))
                {

                    if (count == 0)
                    {
                        actualColumn = kvp.Value.Column;
                        newposition = kvp.Value.Position;
                    }

                    count++;
                }
                else // dziura sie skonczyla
                {
                    if (count > 0)
                    {
                        // znajdz półkę wyzej...
                        foreach (KeyValuePair<int, Tray> kvpt in trayCfg.OrderBy(x => x.Value.Position))
                        {
                            if (kvpt.Value.Column == actualColumn && kvpt.Value.Position >= newposition + count && kvpt.Value.Height <= count)
                            {
                                rv = new TrayToMove();
                                rv.Tray = kvpt.Value.Copy();
                                rv.NewPosition = newposition;
                                rv.NewPositionColumn = actualColumn;
                                break;

                            }
                        }

                    }
                    if (rv != null)
                    {
                        rv.NewPosition = newposition;
                        rv.NewPositionColumn = actualColumn;
                        break;
                    }
                    count = 0;
                    newposition = 0;
                    actualColumn = 0;
                }
            }

            return rv;
        }

        /// <summary>
        /// szuka dziury u probuje pole powyzej dziury wstawic w to miejsce...
        /// </summary>
        /// <param name="posin"></param>
        /// <param name="trays"></param>
        /// <returns></returns>
        private static TrayToMove GetShelfForRemoveEmptySpace_MoveDownOnlyNextOne(Dictionary<int, TrayPosition> posin, Dictionary<int, Tray> trays)
        {
            int newposition = 0;
            int count = 0;
            int actualColumn = 0;

            TrayToMove rv = null;

            Dictionary<int, TrayPosition> positions = new Dictionary<int, TrayPosition>(posin);
            Dictionary<int, Tray> trayCfg = new Dictionary<int, Tray>(trays);
            foreach (KeyValuePair<int, TrayPosition> kvp in positions.OrderBy(a => a.Value))
            {
                if ((kvp.Value.Tray <= 0
                    && (kvp.Value.PositionType == 1 || kvp.Value.PositionType == 2 && count > 0))
                    && (kvp.Value.Column == actualColumn || actualColumn == 0))
                {

                    if (count == 0)
                    {
                        actualColumn = kvp.Value.Column;
                        newposition = kvp.Value.Position;
                    }

                    count++;
                }
                else // dziura sie skonczyla
                {
                    if (count > 0)
                    {
                        // znajdz półkę wyzej...
                        foreach (KeyValuePair<int, Tray> kvpt in trayCfg.OrderBy(x => x.Value.Position))
                        {
                            if (kvpt.Value.Column == actualColumn && kvpt.Value.Position >= newposition)// + count && kvpt.Value.Height <= count)
                            {
                                int[] tt = GetAvaiablePositionsKeysForTray(positions, kvpt.Value.Copy());
                                foreach (int k in tt)
                                {
                                    if (positions.ContainsKey(k))
                                    { // ZOPTYMALIZOWAC!! (napisac szukanie pozycji...)
                                        if (positions[k].Column == actualColumn && positions[k].Position == newposition)
                                        {
                                            rv = new TrayToMove();
                                            rv.Tray = kvpt.Value.Copy();
                                            rv.NewPosition = newposition;
                                            rv.NewPositionColumn = actualColumn;
                                        }
                                    }
                                }

                                break;

                            }
                        }

                    }
                    if (rv != null)
                    {
                        rv.NewPosition = newposition;
                        rv.NewPositionColumn = actualColumn;
                        break;
                    }
                    count = 0;
                    newposition = 0;
                    actualColumn = 0;
                }
            }

            return rv;





            return rv;
        }


        public static TrayToMove GetShelfForRemoveEmptySpace(Dictionary<int, TrayPosition> posin, Dictionary<int, Tray> trays)
        {

            TrayToMove rv = null;
            if (rv == null)
            {
                rv = GetShelfForRemoveEmptySpace_GetTop(posin, trays); // pobor z gory w dol
            }

            //if (rv == null)
            //{
            //    rv = GetShelfForRemoveEmptySpace_MoveDown(posin, trays); // wszystko normalnie w dol ale zmienia regały... 
            //}

            if (rv == null)
            {
                rv = GetShelfForRemoveEmptySpace_MoveDownOnlyNextOne(posin, trays); // wszystko normalnie w dol ale zmienia regały... 
            }

            return rv;

        }
    }
}
