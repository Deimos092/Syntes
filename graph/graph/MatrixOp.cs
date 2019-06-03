using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graph
{
	class MatrixOp
	{
		int[,] _matrix;
		private int _row, _col;
		private string _name;
		//----------------------------------------Construction-------------------------------------------
		//-----------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание пустой матрицы с 0 строк и 0 столбцов
		/// </summary>
		public MatrixOp() : this(0 , 0) { }
		/// <summary>
		/// Инициализация матрицы
		/// </summary>
		/// <param name="row">Количество строк</param>
		/// <param name="col">Количество столбцов</param>
		/// <param name="name">Имя матрицы или по умолчанию (Matrix)</param>
		public MatrixOp( int row , int col , string name = "" )
		{
			_matrix = new int[row , col];
			_row = row;
			_col = col;
			Name = name;
		}
		//----------------------------------------Property-----------------------------------------------
		//-----------------------------------------------------------------------------------------------
		/// <summary>
		/// Строка
		/// </summary>
		public int Row
		{
			get { return _row; }
			private set { _row = ( value >= 0 ) ? value : 0; }
		}
		/// <summary>
		/// Столбец
		/// </summary>
		public int Col
		{
			get { return _col; }
			private set { _col = ( value >= 0 ) ? value : 0; }
		}
		/// <summary>
		/// Изменить количество строк на *
		/// </summary>
		public int ReRow
		{
			set { _matrix = Resize(new int[( value >= 0 ) ? value : 0 , Col]); }
		}
		/// <summary>
		/// Изменить количество столбцов на *
		/// </summary>
		public int ReCol
		{
			set { _matrix = Resize(new int[Row , ( value >= 0 ) ? value : 0]); }
		}
		/// <summary>
		/// Изменить имя матрицы
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = ( value != "" ) ? value : "Matrix"; }
		}
		public int Count { get { return Row * Col; } }
		/// <summary>
		/// Находит минимальный элемент матрицы
		/// </summary>
		public int Min { get { return _matrix.Cast<int>().Min(); } }
		/// <summary>
		/// Находит максимальный элемент матрицы
		/// </summary>
		public int Max { get { return _matrix.Cast<int>().Max(); } }
		/// <summary>
		/// Обращение по индексам к значению
		/// </summary>
		/// <param name="r">Строка</param>
		/// <param name="c">Столбец</param>
		/// <returns></returns>
		public int this[int r , int c]
		{
			get { return _matrix[r , c]; }
			set { _matrix[r , c] = value; }
		}
		public bool IsSquare
		{
			get { return ( Row == Col ) ? true : false; }
		}
		public int GetDimenssion
		{
			get
			{
				if ( IsSquare ) return Row;
				return -1;
			}
		}
		//public static MatrixOp operator *( MatrixOp a , MatrixOp b )
		//{
		//	return a.Multyply(a, b);
		//}
		public static MatrixOp operator *( MatrixOp a , int value )
		{
			return a.Multyply(value);
		}
		public static MatrixOp operator +( MatrixOp a , MatrixOp b )
		{
			return a.Additive(a , b);
		}
		public static MatrixOp operator +( MatrixOp a , int value )
		{
			return a.Additive(value);
		}
		public static MatrixOp operator -( MatrixOp a , MatrixOp b )
		{
			return a.Subtract(a , b);
		}
		public static MatrixOp operator -( MatrixOp a , int value )
		{
			return a.Subtract(value);
		}
		//----------------------------------------Methods------------------------------------------------
		//-----------------------------------------------------------------------------------------------
		private int[,] Resize( int[,] _M )
		{
			for ( int i = 0 ; i < _M.GetLength(0) ; i++ )
				for ( int j = 0 ; j < _M.GetLength(1) ; j++ )
					_M[i , j] = ( i < Row & j < Col ) ? this[i , j] : 0;

			Row = _M.GetLength(0);
			Col = _M.GetLength(1);
			return _M;
		}
		/// <summary>
		/// Заполнить матрицу случайными числами 
		/// </summary>
		/// <param name="max">Максимальная граница</param>
		/// <param name="min">Минимальная граница</param>
		public void RndFill( int max = 100 , int min = 0 )
		{
			Random rnd = new Random();
			for ( int i = 0 ; i < Row ; i++ )
				for ( int j = 0 ; j < Col ; j++ )
					this[i , j] = rnd.Next(min , max);
		}
		/// <summary>
		/// Сложение текущей матрицы с числом А
		/// </summary>
		/// <param name="value">Число А</param>
		/// <returns>Возвращает текущую матрицу</returns>
		public MatrixOp Additive( int value )
		{
			for ( int i = 0 ; i < Row ; i++ )
				for ( int j = 0 ; j < Col ; j++ )
					this[i , j] += value;
			return this;
		}
		/// <summary>
		/// Сложение Матрицы А с Матрицей В
		/// </summary>
		/// <param name="Amatrix">Матрица А</param>
		/// <param name="Bmatrix">Матрица В</param>
		/// <returns>Возвращает Матрицу А</returns>
		public MatrixOp Additive( MatrixOp Amatrix , MatrixOp Bmatrix )
		{
			if ( !Amatrix.Row.Equals(Bmatrix.Row) && !Amatrix.Col.Equals(Bmatrix.Col) ) throw new Exception("Матрицы нельзя сложить");

			for ( int i = 0 ; i < Amatrix.Row ; i++ )
				for ( int j = 0 ; j < Amatrix.Col ; j++ )
					Amatrix[i , j] += Bmatrix[i , j];
			return Amatrix;
		}
		/// <summary>
		/// Вычиатние из текущей матрицы число А
		/// </summary>
		/// <param name="value">Число А</param>
		/// <returns>Возвращает текущую матрицу</returns>
		public MatrixOp Subtract( int value )
		{
			for ( int i = 0 ; i < Row ; i++ )
				for ( int j = 0 ; j < Col ; j++ )
					this[i , j] -= value;
			return this;
		}
		/// <summary>
		/// Вычитание из матрицы А матрицу В
		/// </summary>
		/// <param name="Amatrix">Матрица А</param>
		/// <param name="Bmatrix">Матрица В</param>
		/// <returns>Возвращает Матрицу А из которой вычли Матрицу В</returns>
		public MatrixOp Subtract( MatrixOp Amatrix , MatrixOp Bmatrix )
		{
			if ( !Amatrix.Row.Equals(Bmatrix.Row) && !Amatrix.Col.Equals(Bmatrix.Col) ) throw new Exception("Матрицы нельзя вычесть");

			for ( int i = 0 ; i < Amatrix.Row ; i++ )
				for ( int j = 0 ; j < Amatrix.Col ; j++ )
					this[i , j] -= Bmatrix[i , j];
			return this;
		}
		/// <summary>
		/// Умножение текущей матрицы с матрицей А
		/// </summary>
		/// <param name="matrix">Матрица А</param>
		public void Multyply( MatrixOp matrix )
		{
			if ( !Col.Equals(matrix.Row) ) throw new Exception("Матрицы нельзя перемножить");

			int[,] _result = new int[Row , matrix.Col];

			for ( int i = 0 ; i < Row ; i++ )
				for ( int j = 0 ; j < matrix.Col ; j++ )
					for ( int k = 0 ; k < matrix.Row ; k++ )
						_result[i , j] += this[i , k] * matrix[k , j];

			_matrix = _result;
		}
		/// <summary>
		/// Умножение Матрицы А с Матрицей В
		/// </summary>
		/// <param name="A_matrix">Матрица А</param>
		/// <param name="B_matrix">Матрица В</param>
		/// <returns>Возвращает новую матрицу С </returns>
		public void Multyply( MatrixOp A_matrix , MatrixOp B_matrix )
		{
			if ( !A_matrix.Col.Equals(B_matrix.Row) ) throw new Exception("Матрицы нельзя перемножить");

			for ( int i = 0 ; i < A_matrix.Row ; i++ )
				for ( int j = 0 ; j < B_matrix.Col ; j++ )
					for ( int k = 0 ; k < B_matrix.Row ; k++ )
						this[i , j] += A_matrix[i , k] * B_matrix[k , j];
		}
		/// <summary>
		/// Умножение текущей матрицы с числом А
		/// </summary>
		/// <param name="value">Число А</param>
		/// <returns></returns>
		public MatrixOp Multyply( int value )
		{
			for ( int i = 0 ; i < Row ; i++ )
				for ( int j = 0 ; j < Col ; j++ )
					this[i , j] *= value;
			return this;
		}
		
		/// <summary>
		/// Транспонирует матрицу меняет местами строки со столбцами
		/// </summary>
		public MatrixOp Transperate()
		{
			MatrixOp matrixOp = new MatrixOp(Row, Col);
			MatrixOp tmp = this;
			for ( int i = 0 ; i < Row ; i++ )
				for ( int j = 0 ; j < Col ; j++ )
					matrixOp[i , j] = tmp[j , i];
			return matrixOp;
		}
		
	}
}
