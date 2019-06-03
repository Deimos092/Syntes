using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace graph
{
	public partial class Main_Form : Form
	{
		public Main_Form()
		{
			InitializeComponent();
		}
		//--------------------------- Свойства -----------------------------------
		private MatrixOp Matrix_A { get; set; }
		private MatrixOp Matrix_AT { get; set; }
		private MatrixOp MultyplyMatrix { get; set; }
		private MatrixOp MatrixCollision { get; set; }
		private List<int> CoefficentsList { get; set; }
		private List<Connection> LeftEl { get; set; }
		public List<string> TopEl { get; set; }
		private List<MCollision> Matrix_Collision { get; set; }
	
		//------------------------------------------------------------------------
		int GetIndexMaxValue( int index )
		{
			if ( index != -1 & Matrix_Collision[index].Value.Sum() != 0 )
			{
				int newindex = Matrix_Collision[index].Value.FindIndex(p => p == Matrix_Collision[index].Value.Max());//Находим индекс макс числа в матрице по строке
				Matrix_Collision[index].Value[newindex] = 0;//Изменяем на 0 что это макс число больше не повторялось
				return newindex;//Возвращаем новый индекс обратно
			}
			else
				return -1;
		}
		void GetResize( ref List<MConResult> Lists , MConResult Object , int HowMuch )
		{
			for ( int i = 1 ; i < HowMuch ; i++ )
				Lists.Add(new MConResult(Object.Name,Object.Value));
		}
		int GetNewIndex(int index,List<int> IndexListing)
		{
			while ( index != -1 & IndexListing.Contains(index) )
				index = GetIndexMaxValue(index);
			return index;
		}
		private void Calculate( MConResult _list , ref List<int> IndexListing , int index , int HowMuch = 1 )
		{
			while ( index != -1 )
			{
				_list.Value += Matrix_Collision[index].Value.Max();//Суммируем значения в список
				_list.Name += string.Format($"{Matrix_Collision[index].Name,-3}--> ");//Добавляем элемент в список
				IndexListing.Add(index);//Добавляем индекс что бы он не повторялся

				HowMuch = Matrix_Collision[index].Value.Count(p => p == Matrix_Collision[index].Value.Max());//Проверка на кол-во одинаковых макс значений
				if ( HowMuch > 1 ) break;

				while ( index != -1 & IndexListing.Contains(index) )
					index = GetIndexMaxValue(index);//Берем следующий индекс
			}
			Rtb_1.Text += $"\n{_list.Name}{_list.Value}";

		}
		private void CalcResult( ref List<MConResult> ListsResult , List<int> IndexListing , int index , List<int> IndexPoint, int HowMuch )
		{
			foreach ( MConResult _list in ListsResult )
			{
				Calculate(_list , ref IndexListing , index , HowMuch);//Функция где добавляем элемент в список

				HowMuch = Matrix_Collision[index].Value.Count(p => p == Matrix_Collision[index].Value.Max());//Проверка на кол-во одинаковых макс значений
				if ( HowMuch > 1)
				{
					while ( index != -1 & IndexListing.Contains(index) )
						index = GetIndexMaxValue(index);//Берем следующий индекс 

					if ( index != -1 )
					{
						GetResize(ref ListsResult , _list , HowMuch);//Добавляем еше списки в зависимости от кол-ва HowMuch
						CalcResult(ref ListsResult , IndexListing , index , IndexPoint, HowMuch);//Рекурсивно вызываем функцию с измененным массивом списков
					}
					
				}
				
				break;
			}
		
		}

		private void Result()
		{
			List<MConResult> ListsResult = new List<MConResult>();

			int index = 0, HowMuch = 0;

			HowMuch = CoefficentsList.Count(p => p == CoefficentsList.Min());//Смотрим сколько одинаковых значений есть
			index = CoefficentsList.FindIndex(p => p == CoefficentsList.Min());//Берем его индекс
			CoefficentsList[index] = CoefficentsList.Max();//Изменяем его что бы больше не попадался

			if ( HowMuch > 1 )
				GetResize(ref ListsResult , new MConResult() , HowMuch);
			else
				ListsResult.Add(new MConResult());

			List<int> IndexPoint = new List<int>();//Индекс элемента(точки) на которых наш список разделяется на 2 и более
			List<int> IndexListing = new List<int>();//Индексы которые мы берем для создания списка 
			CalcResult(ref ListsResult , IndexListing , index , IndexPoint, HowMuch);
			//Rtb_1.Text += $"\n{ListsResult[0].Name}{ListsResult[0].Value}";
		}
		List<int> GetCoefficient( MatrixOp matrix )
		{
			int count = 0;
			List<int> myCoefficient = new List<int>();

			for ( int i = 0 ; i < matrix.Row ; i++ )
			{
				for ( int j = 0 ; j < matrix.Col ; j++ )
					if ( matrix[i , j] > 0 ) count++;
				myCoefficient.Add(count);
				count = 0;
			}
			return myCoefficient;
		}
		private void button1_Click( object sender , EventArgs e )
		{
			openFileDialog1.Title = "Загрузите файл с расширением .net";
			openFileDialog1.FileName = "Загрузите файл";
			openFileDialog1.Filter = ".net|*.net|.all|*.*";
			string fname;

			if ( openFileDialog1.ShowDialog() != DialogResult.OK )
				return;
			fname = openFileDialog1.FileName; // */

			textBox1.Text = System.IO.File.ReadAllText(fname);

			parse();//Парсим исходный файл
			calcMatr();//Вычисляем все матрицы А АТ и перемножение
			CoefficentsList = GetCoefficient(MultyplyMatrix);//Вывод списка коэффицентов
			Tab_3(sender , e);//Вызов события
			Result();//Вычисления основного алгоритма для создания списков СССУУУККАААА!!!
		}

		void parse()
		{
			LeftEl = new List<Connection>();
			TopEl = new List<string>();

			var CountLine = from line in textBox1.Lines
							where line == "$NETS"
							select Array.IndexOf(textBox1.Lines , line) + 1;
			int i = CountLine.First();

			Connection Connector = null;
			Matrix_Collision = new List<MCollision>();
			string MainString, s2;

			for ( int c = 0 ; ( i < textBox1.Lines.Count() ) && ( textBox1.Lines[i] != "$END" ) ; i++ )
			{
				MainString = textBox1.Lines[i];//Загружаем после $NETS 29 ячейка

				int IndexPosition = MainString.IndexOf(";");
				if ( IndexPosition > 0 )
				{
					Connector = new Connection();
					Connector.name = MainString.Substring(0 , IndexPosition);
					MainString = MainString.Substring(IndexPosition + 1);

					LeftEl.Add(Connector);

				}

				string[] StrArray = MainString.TrimStart().Split(' ');
				for ( int j = 0 ; j < StrArray.Count() ; j++ )
				{
					s2 = StrArray[j].Trim();
					if ( s2.Length == 0 ) continue;

					IndexPosition = s2.IndexOf(".");
					if ( IndexPosition > 0 )
						s2 = s2.Substring(0 , IndexPosition);

					Connector.to.Add(s2);
					if ( TopEl.IndexOf(s2) < 0 )
					{
						TopEl.Add(s2);
						Matrix_Collision.Add(new MCollision());
						Matrix_Collision[c++].Name = s2;
					}
				}
			}

			string Result = "";
			for ( i = 0 ; i < LeftEl.Count() ; i++ )
				Result += string.Format($"{LeftEl[i].name,-8}");
			textBox2.AppendText($"{"Left:",-6}{Result}\n");

			Result = "";
			for ( i = 0 ; i < TopEl.Count() ; i++ )
				Result += string.Format($"{TopEl[i],-8}");
			textBox2.AppendText($"{"Top:",-5}{Result}\n");
			textBox2.AppendText(new string('~' , 100) + "\n");
		}

		//Подсчет положительных элементов
		int GetCount( int FirstElement , int SecondElement , MatrixOp matrix )
		{
			int count = 0;
			for ( int i = 0 ; i < matrix.Col ; i++ )
			{
				if ( matrix[FirstElement , i] * matrix[SecondElement , i] != 0 )
					count++;
			}
			return count;
		}
		void GetCountCollision( MatrixOp matrix )
		{
			for ( int i = 0 ; i < matrix.Row ; i++ )
			{
				for ( int j = 0 ; j < matrix.Col ; j++ )
				{
					if ( i != j )
						Matrix_Collision[i].Value.Add(GetCount(i , j , matrix));
				}
			}
			
		}
		void calcMatr()
		{

			int size = Math.Max(TopEl.Count() , LeftEl.Count());

			Matrix_A = new MatrixOp(size , size , "Матрица_A");
			Matrix_AT = new MatrixOp(size , size);
			MultyplyMatrix = new MatrixOp(size , size , "Умножение A * AT");

			for ( int i = 0 ; i < LeftEl.Count ; i++ )
				for ( int j = 0 ; j < LeftEl[i].to.Count ; j++ )
				{
					int value = TopEl.IndexOf(LeftEl[i].to[j]);
					Matrix_A[value , i] = 1;
				}
			Matrix_AT = Matrix_A.Transperate();//Транспонируем матрицу
			Matrix_AT.Name = "Матрица_AT";//Даем имя

			MultyplyMatrix.Multyply(Matrix_A , Matrix_AT);//Перемножение матриц

			GetCountCollision(MultyplyMatrix);//Создаем матрицу коллизий т.е. где эл наши встречаются
			//------------------------------------------------------------------------------
			string mystring;
			textBox2.AppendText($"\n{Matrix_A.Name}\n");
			textBox2.AppendText("\t" + string.Join("\t" , LeftEl.Select(v => v.name)) + "\n");
			for ( int i = 0 ; i < size ; i++ )
			{
				mystring = TopEl[i] + "\t";
				for ( int j = 0 ; j < size ; j++ )
					mystring += Matrix_A[i , j] + "\t";
				textBox2.AppendText(mystring + "\n");
			}
			//------------------------------------------------------------------------------
			textBox2.AppendText($"\n{Matrix_AT.Name}\n");
			textBox2.AppendText("\t" + string.Join("\t" , LeftEl.Select(v => v.name)) + "\n");
			for ( int i = 0 ; i < size ; i++ )
			{
				mystring = TopEl[i] + "\t";
				for ( int j = 0 ; j < size ; j++ )
					mystring += Matrix_AT[i , j] + "\t";
				textBox2.AppendText(mystring + "\n");
			}
			//------------------------------------------------------------------------------
			textBox2.AppendText($"\n{MultyplyMatrix.Name}\n");
			textBox2.AppendText("\t" + string.Join("\t" , LeftEl.Select(v => v.name)) + "\n");
			for ( int i = 0 ; i < size ; i++ )
			{
				mystring = TopEl[i] + "\t";
				for ( int j = 0 ; j < size ; j++ )
					mystring += MultyplyMatrix[i , j] + "\t";
				textBox2.AppendText(mystring + "\n");
			}
			this.Matrix_A = Matrix_A;
		}

		class Connection
		{
			public string name;

			public List<string> to = new List<string>();
		}
		class MCollision
		{
			public string Name;
			public List<int> Value = new List<int>();
		}
		class MConResult
		{
			public string Name;
			public int Value;
			public MConResult() { }
			public MConResult(string name,int val)
			{
				Name = name;
				Value = val;
			}
		}
		private void Form1_Load( object sender , EventArgs e )
		{

		}

		float m, scrr;
		int cx, cy;

		private void Tab_3( object sender , EventArgs e )
		{
			StringBuilder _string = new StringBuilder();
			_string.AppendLine($"- - - - - - - - - - - - - - - Матрица Коллизий - - - - - - - - - - - - - - -");
			for ( int i = 0 ; i < Matrix_Collision.Count ; i++ )
			{
				_string.Append($"{i + 1,3:0#}:");
				for ( int j = 0 ; j < Matrix_Collision[i].Value.Count ; j++ )
					_string.Append($"{Matrix_Collision[i].Value[j],5:0#}");
				_string.AppendLine();
			}


			_string.AppendLine($"Взяли коэффицент из матрицы ({MultyplyMatrix.Name})");
			for ( int i = 0 ; i < CoefficentsList.Count ; i++ )
				_string.AppendFormat("{0,-3}" , CoefficentsList[i]);

			Rtb_1.Text = _string.ToString();
		}

		void calcPt( ref Point pt , int i )
		{
			pt.X = (int)( cx + Math.Cos(i * m) * scrr );
			pt.Y = (int)( cy + Math.Sin(i * m) * scrr );
		}

		private void pictureBox1_Paint( object sender , PaintEventArgs e )
		{
			int i, j, k;
			Graphics gr = e.Graphics;
			Point pt = new Point(), pt2 = new Point();

			if ( Matrix_A == null ) return;

			cx = pictureBox1.Width / 2;
			cy = pictureBox1.Height / 2;

			scrr = Math.Min(cx , cy) - 30;

			m = (float)( 2.0f * Math.PI / TopEl.Count() );

			for ( j = 0 ; j < TopEl.Count ; j++ )
			{
				calcPt(ref pt , j);

				for ( i = j + 1 ; i < TopEl.Count ; i++ )
				{
					// Перебираем все дорожки и ищем соединение
					bool fl = false;
					for ( k = 0 ; ( k < LeftEl.Count ) & !fl ; k++ )
						if ( ( Matrix_A[k , i] == 1 ) & ( Matrix_A[k , j] == 1 ) )
							fl = true;

					if ( !fl ) continue;

					calcPt(ref pt2 , i);

					gr.DrawLine(Pens.Black , pt , pt2);
				}
			}

			for ( j = 0 ; j < TopEl.Count ; j++ )
			{
				calcPt(ref pt , j);

				if ( pt.X < cx )
					pt.X -= 70;
				else
					pt.X -= 10;

				gr.FillRectangle(Brushes.White , pt.X , pt.Y - 8 , 80 , 18);
				gr.DrawRectangle(Pens.Black , pt.X , pt.Y - 8 , 80 , 18);
				gr.DrawString(TopEl[j] , this.Font , Brushes.Black , pt.X + 1 , pt.Y - 8 + 1);
			}
		}
	}
}

