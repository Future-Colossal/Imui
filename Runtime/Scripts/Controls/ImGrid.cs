using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Imui.Core;
using UnityEngine;

namespace Imui.Controls
{
    [StructLayout(LayoutKind.Sequential)]
    public ref struct ImGridState
    {
        public Vector2 Origin;
        public Vector2 CellSize;
        public Vector2 Spacing;
        public int Columns;
        public int X;
        public int Y;
        
        /// <summary>
        /// A value that is internally updated to give the current "Height" of the grid.
        /// There is no guarantee that a cell at this height was actually rendered - this only keeps track
        /// of cell queries
        /// </summary>
        public int MaxRowReferenced;
        
        // indexer
        // todo: with modern .NET, this should be converted into an extension indexer (is that supported?)
        public ImRect this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ImGrid.GetGridCell(null, ref this, x, y);
        }
    }

    public static class ImGrid
    {
        public static ImGridState BeginGrid(this ImGui gui, int columns, float cellHeight = 0)
        {
            var width = gui.GetLayoutWidth();
            var spacing = GetDefaultSpacing(gui);
            var cellWidth = Mathf.Floor((width + spacing.x) / columns - spacing.x);
            cellHeight = cellHeight <= 0 ? cellWidth : cellHeight;

            return BeginGrid(gui, new Vector2(cellWidth, cellHeight), spacing);
        }

        public static ImGridState BeginGrid(this ImGui gui, Vector2 cellSize)
        {
            return BeginGrid(gui, cellSize, GetDefaultSpacing(gui));
        }

        public static ImGridState BeginGrid(this ImGui gui, Vector2 cellSize, Vector2 spacing)
        {
            ref readonly var frame = ref gui.Layout.GetFrame();

            var width = gui.Layout.GetAvailableWidth();
            var columns = Mathf.Max(1, (width + spacing.x) / (cellSize.x + spacing.x));
            var state = new ImGridState()
            {
                Origin = ImLayout.GetNextPosition(in frame, 0),
                CellSize = cellSize,
                Columns = (int)columns,
                Spacing = spacing
            };

            return state;
        }

        public static void EndGrid(this ImGui gui, in ImGridState state)
        {
            var width = state.Columns * state.CellSize.x + (state.Columns - 1) * state.Spacing.x;
            var height = (state.Y + 1) * state.CellSize.y + state.Y * state.Spacing.y;
            gui.Layout.AddRect(width, height);
        }

        /// <summary>
        /// Moves to the "next" grid position, first horizontally (column/X), then vertically (row/Y).
        /// </summary>
        /// <param name="state"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveNext(ref this ImGridState state)
        {
            if (++state.X >= state.Columns)
            {
                state.X = 0;
                ++state.Y;
            }
        }
        

        /// <summary>
        /// Moves to the next column, or wraps back to the first column
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public static void MoveNextColumnX(ref this ImGridState state)
        {
            state.X = state.IsAtEndOfRow() ? 0 : state.X + 1;
        }

        /// <summary>
        /// Increments the row value, with no upper limit
        /// </summary>
        /// <param name="state"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveNextRowY(ref this ImGridState state) => ++state.Y;

        /// <summary>
        /// Moves to the next cell and returns its rect. <seealso cref="MoveNext"/>
        /// </summary>
        public static ImRect GridNextCell(this ImGui gui, ref ImGridState state)
        {
            state.MoveNext();
            return CreateCellRect(ref state, state.X, state.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtEndOfRow(in this ImGridState state) => state.X >= state.Columns - 1;

        /// <summary>
        /// Returns the cell rect of the state's current coordinates
        /// </summary>
        public static ImRect GetCurrentGridCell(ref this ImGridState state)
        {
            if (state.X < 0 || state.Y < 0 || state.X >= state.Columns)
            {
                throw new IndexOutOfRangeException("Coordinates must be > 0, and xColumn must be < ImGridState.Columns");
            }
            
            return CreateCellRect(ref state, state.X, state.Y);
        }
        

        /// <summary>
        /// Gets an arbitrary grid cell
        /// </summary>
        /// <param name="_">Unused, just here to allow for this convenient extension method</param>
        /// <param name="state">The grid state</param>
        /// <param name="xColumn">The X coordinate, 0-based. Must not equal or exceed <see cref="ImGridState.Columns"/></param>
        /// <param name="yRow">The Y coordinate, 0-based.</param>
        /// <param name="traverse">If true, the state will be updated with the given coordinates.
        /// Otherwise, the state's position will be unaffected hereafter.</param>
        /// <returns>The rect of the given coordinates</returns>
        /// <exception cref="IndexOutOfRangeException">If either coordinate is &lt; 0, or if x is &gt;= <see cref="ImGridState.Columns"/></exception>
        public static ImRect GetGridCell(this ImGui _, ref ImGridState state, int xColumn, int yRow, bool traverse)
        {
            if (xColumn < 0 || yRow < 0 || xColumn >= state.Columns)
            {
                throw new IndexOutOfRangeException("Coordinates must be > 0, and xColumn must be < ImGridState.Columns");
            }

            if (traverse)
            {
                state.X = xColumn;
                state.Y = yRow;
            }
            
            return CreateCellRect(ref state, xColumn, yRow);
        }
        
        /// <summary>
        /// Gets an arbitrary grid cell
        /// </summary>
        /// <param name="_">Unused, just here to allow for this convenient extension method</param>
        /// <param name="state">The grid state</param>
        /// <param name="xColumn">The X coordinate, 0-based. Must not equal or exceed <see cref="ImGridState.Columns"/></param>
        /// <param name="yRow">The Y coordinate, 0-based.</param>
        /// <returns>The rect of the given coordinates</returns>
        /// <exception cref="IndexOutOfRangeException">If either coordinate is &lt; 0, or if x is &gt;= <see cref="ImGridState.Columns"/></exception>
        public static ImRect GetGridCell(this ImGui _, ref ImGridState state, int xColumn, int yRow)
        {
            if (xColumn < 0 || yRow < 0 || xColumn >= state.Columns)
            {
                throw new IndexOutOfRangeException("Coordinates must be > 0, and xColumn must be < ImGridState.Columns");
            }
            
            return CreateCellRect(ref state, xColumn, yRow);
        }

        private static ImRect CreateCellRect(ref ImGridState state, int xColumn, int yRow)
        {
            state.MaxRowReferenced = System.Math.Max(yRow, state.MaxRowReferenced);
            CalculateCellPosition(xColumn, yRow, state.CellSize, state.Spacing, state.Origin, out var pos);
            return new ImRect(pos.x, pos.y - state.CellSize.y, state.CellSize.x, state.CellSize.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CalculateCellPosition(int coordX, int coordY, in Vector2 cellSize, in Vector2 spacing,
            in Vector2 origin, out Vector2 pos)
        {
            pos = new Vector2(
                x: origin.x + coordX * (cellSize.x + spacing.x), 
                y: origin.y - coordY * (cellSize.y + spacing.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetDefaultSpacing(this ImGui gui) => new(gui.Style.Layout.Spacing, gui.Style.Layout.Spacing);
    }
}