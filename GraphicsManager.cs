using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace AnimatedText
{
// класс для работы с трехмерной графикой в приложении
public class GraphicsManager : IDisposable
{
	// ширина и высота текстуры с текстом
	private const int TEXTURE_WIDTH = 256;
	private const int TEXTURE_HEIGHT = 128;

	// флаги состояния для вращения по осям X, Y, Z
	private bool rotateX = true;
	private bool rotateY = true;
	private bool rotateZ = true;

	// скорость вращения текстуры
	private float rotationSpeed = 1.0f;
	// угол вращения текстуры
	private float rotationAngle = 0.0f;

	// ID текстуры
	private int textureId;
	public int TextureId => textureId;

	// инициализация OpenGL
	public void Initialize(int width, int height)
	{
		// цвета очистки экрана (значения для буфера цвета)
		GL.ClearColor(Color.Black);

		// включение поддержки 2D текстур
		GL.Enable(EnableCap.Texture2D);

		// установка области просмотра
		GL.Viewport(0, 0, width, height);

		// задание матрицы проекции в качестве текущей матрицы
		GL.MatrixMode(MatrixMode.Projection);
		// сброс текущей матрицу
		GL.LoadIdentity();

		// угол обзора (60 градусов)
		float fov = MathHelper.PiOver3;

		// предотвращение деления на ноль
		if (height == 0)
			height = 1;

		// соотношение сторон
		float aspectRatio = (float)width / (float)height;
		// ближняя плоскость отсечения
		float zNear = 0.5f;
		// дальняя плоскость отсечения
		float zFar = 500f;

		// задание перспективной проекции
		Matrix4 perspMat = Matrix4.CreatePerspectiveFieldOfView(
			fov,
			aspectRatio,
			zNear,
			zFar);
		GL.LoadMatrix(ref perspMat);
	}

	// создание текстуры с текстом
	public void GenerateTexture(string text)
	{
		// удаление прежней текстуры, если была сгенерирована новая
		if (textureId > 0)
			GL.DeleteTextures(1, ref textureId);

		// генерация новой текстуры
		GL.GenTextures(1, out textureId);
		// создание именованной текстуры, привязанной к целевому объекту текстуры
		GL.BindTexture(TextureTarget.Texture2D, textureId);

		// параметры текстурирования
		GL.TexParameter(
			TextureTarget.Texture2D,
			TextureParameterName.TextureMinFilter,
			(float)TextureMinFilter.Nearest);

		GL.TexParameter(
			TextureTarget.Texture2D,
			TextureParameterName.TextureMagFilter,
			(float)TextureMagFilter.Nearest);

		// обновление текста на текстуре
		UpdateTexture(text);
	}

	// анимация текстуры
	public void AnimateTexture()
	{
		// очистка буферов цвета и глубины
		GL.Clear(
			ClearBufferMask.ColorBufferBit |
			ClearBufferMask.DepthBufferBit);

		// задание модельно-видовой матрицы качестве текущей
		GL.MatrixMode(MatrixMode.Modelview);
		// сброс текущей матрицу
		GL.LoadIdentity();

		// смещение текстуры по оси Z
		GL.Translate(0, 0, -5.0f);

		// вращение текстуры
		RotateTexture();
	}

	// вращение текстуры
	public void RotateTexture()
	{
		// увеличение угла вращения
		rotationAngle += rotationSpeed;

		// сброс угла вращения при полном обороте
		if (rotationAngle > 360.0f)
			rotationAngle = 0.0f;

		if (rotateX)
			// вращение по оси X
			GL.Rotate(rotationAngle, 1.0f, 0.0f, 0.0f);
		if (rotateY)
			// вращение по оси Y
			GL.Rotate(rotationAngle, 0.0f, 1.0f, 0.0f);
		if (rotateZ)
			// вращение по оси z
			GL.Rotate(rotationAngle, 0.0f, 0.0f, 1.0f);
	}

	// отрисовка текстуры
	public void DrawTexturedQuad()
	{
		// использование ранее созданной текстуры для отрисовки
		GL.BindTexture(TextureTarget.Texture2D, textureId);

		// Начало отрисовки с использованием прямоугольников
		// в качестве двумерных примитивов
		GL.Begin(PrimitiveType.Quads);

		// текстурные координаты и координаты вершины
		GL.TexCoord2(0.0f, 0.0f);
		GL.Vertex3(-1.0f, -1.0f, 0.0f);
		GL.TexCoord2(1.0f, 0.0f);
		GL.Vertex3(1.0f, -1.0f, 0.0f);
		GL.TexCoord2(1.0f, 1.0f);
		GL.Vertex3(1.0f, 1.0f, 0.0f);
		GL.TexCoord2(0.0f, 1.0f);
		GL.Vertex3(-1.0f, 1.0f, 0.0f);

		// окончание отрисовки
		GL.End();

		// освобождение ранее использованной тектуры
		GL.BindTexture(TextureTarget.Texture2D, 0);
	}

	// освобождение ресурсов
	public void Dispose()
	{
		if (textureId > 0)
			GL.DeleteTextures(1, ref textureId);
	}

	// задание оси для вращения текстуры
	public void ToggleRotationAxisX(bool enable)
	{
		rotateX = enable;
	}

	public void ToggleRotationAxisY(bool enable)
	{
		rotateY = enable;
	}

	public void ToggleRotationAxisZ(bool enable)
	{
		rotateZ = enable;
	}

	// скорость вращения текстуры
	public void SetRotationSpeed(float speed)
	{
		rotationSpeed = speed;
	}

	// обновление текстуры на основе растрового изображения
	private void UpdateTexture(string text)
	{
		Bitmap textBitmap = CreateTextBitmap(text);
		BitmapData data = textBitmap.LockBits(
			new Rectangle(0, 0, textBitmap.Width, textBitmap.Height),
			ImageLockMode.ReadOnly,
			System.Drawing.Imaging.PixelFormat.Format32bppArgb);

		GL.TexImage2D(
			TextureTarget.Texture2D,
			0,
			PixelInternalFormat.Rgba,
			textBitmap.Width,
			textBitmap.Height,
			0,
			OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
			PixelType.UnsignedByte,
			data.Scan0);

		textBitmap.UnlockBits(data);
	}

	// создание растрового изображения с указанным текстом
	private Bitmap CreateTextBitmap(string text)
	{
		Bitmap bitmap = new Bitmap(TEXTURE_WIDTH, TEXTURE_HEIGHT);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.Clear(Color.Black);
			Font drawFont = new Font("Arial", 16);
			graphics.DrawString(
				text,
				drawFont,
				Brushes.White,
				new RectangleF(0, 0, TEXTURE_WIDTH, TEXTURE_HEIGHT));
		}
		return bitmap;
	}
}
}
