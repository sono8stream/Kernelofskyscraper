using System;
﻿using UnityEngine;
using System.Collections;

public static class EaseFunctions {

	/// <summary>
	/// 直線補完型イージング関数
	/// </summary>
	/// <param name="start">始点値</param>
	/// <param name="end">終点値</param>
	/// <param name="current">現在の進行度</param>
	/// <param name="max">進行度の最大値</param>
	/// <param name="overshoot">maxを超えるcurrentが設定された場合に、終点を超えて良い場合はTrue</param>
	/// <param name="undershoot">0を下回るcurrentが設定された場合に、始点を超えて良い場合はTrue</param>
	/// <returns>入力値に基づいた数値が返ってきます</returns>
	public static float Linear(double start, double end, double current, double max, bool overshoot, bool undershoot)
		{

			float rate = (float)(current / max);
			if (!overshoot) {
				rate = rate > 1.0f ? 1.0f : rate;
			}
			if (!undershoot) {
				rate = rate < 0.0f ? 0.0f : rate;
			}
			return (float)(start + (end-start) * rate);

		}

		/// <summary>
		/// 整関数補完型イージング関数(加速)
		/// </summary>
		/// <param name="start">始点値</param>
		/// <param name="end">終点値</param>
		/// <param name="power">指数</param>
		/// <param name="current">現在の進行度</param>
		/// <param name="max">進行度の最大値</param>
		/// <param name="overshoot">maxを超えるcurrentが設定された場合に、終点を超えて良い場合はTrue</param>
		/// <param name="undershoot">0を下回るcurrentが設定された場合に、始点を超えて良い場合はTrue</param>
		/// <returns>入力値に基づいた数値が返ってきます</returns>
		/// <remarks>Linearとの違いは、進行度の計算時にその数値を底としてpower乗することです。overshoot/undershootをtrueにした場合でも整関数補完は効きます。また、アンダーシュートした場合は、補完は現在地の絶対値分をcurrentとした場合と同じように効きます(ただし、逆側に動きます)</remarks>
		public static float EaseIn(double start, double end, double power, double current, double max, bool overshoot, bool undershoot) {

			float rate = 0.0f;
			// rateに非整数を入れると、currentが-の時に虚数の世界に突入してとんでもないことになる
			// そこで、絶対値を利用することとする
			if (!undershoot) {
				rate = current < 0.0f ? 0.0f : (float)Math.Pow((Math.Abs(current) / max), power);
			}
			else {
				rate = (float)Math.Pow((Math.Abs(current) / max), power);
			}
			if (!overshoot) {
				rate = rate > 1.0f ? 1.0f : rate;
			}

			return (float)(start + (end-start) * rate * (current >= 0 ? 1 : -1));

		}

		/// <summary>
		/// 整関数補完型イージング関数(減速)
		/// </summary>
		/// <param name="start">始点値</param>
		/// <param name="end">終点値</param>
		/// <param name="power">指数</param>
		/// <param name="current">現在の進行度</param>
		/// <param name="max">進行度の最大値</param>
		/// <param name="overshoot">maxを超えるcurrentが設定された場合に、終点を超えて良い場合はTrue</param>
		/// <param name="undershoot">0を下回るcurrentが設定された場合に、始点を超えて良い場合はTrue</param>
		/// <returns>入力値に基づいた数値が返ってきます</returns>
		/// <remarks>Linearとの違いは、進行度の計算時にその数値を底としてpower乗することです。overshoot/undershootをtrueにした場合でも整関数補完は効きます。また、アンダーシュートした場合は、補完は現在地の絶対値分をcurrentとした場合と同じように効きます(ただし、逆側に動きます)</remarks>
		public static float EaseOut(double start, double end, double power, double current, double max, bool overshoot, bool undershoot) {

			float rate = 0.0f;
			// rateに非整数を入れると、currentが-の時に虚数の世界に突入してとんでもないことになる
			// そこで、絶対値を利用することとする
			if (!undershoot) {
				rate = current < 0.0f ? 0.0f : (float)Math.Pow((Math.Abs(current) / max), 1 / power);
			}
			else {
				rate = (float)Math.Pow((Math.Abs(current) / max), 1 / power);
			}
			if (!overshoot) {
				rate = rate > 1.0f ? 1.0f : rate;
			}

			return (float)(start + (end-start) * rate * (current >= 0 ? 1 : -1));

		}

}
