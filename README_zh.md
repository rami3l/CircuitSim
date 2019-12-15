# CircuitSim

一个 SPICE 的缩水实现，作为一个基于 Unity 的电路模拟器项目后端库。

## 项目结构

### library
核心业务代码部分，包含以下功能：
- 多模式电源，提供直流、正弦、阶跃、方波四种信号输出模式
  + 为了电路分析方便，提供电流源与电压源两种电源
  + 根据 SPICE 原理，接入电路的电源数量不受限制
- 目前只实现了电压探针，可以分析电路中指定节点相对于电路接地处的电压
- 基于纯电阻电路配置的 [DC 分析](https://github.com/rami3l/CircuitSim/blob/master/library/DCAnalysis.cs)
  + 由于前端实现的限制，DC 分析在模拟器中并未直接使用，而是作为瞬态分析的步骤之一进行调用
- 基于电阻、电感器、电容器的基本电路配置的[瞬态分析](https://github.com/rami3l/CircuitSim/blob/master/library/TransientAnalysis.cs)

### test-library
基本单元测试

### app
与前端代码对接之前的绘图测试，用于验证暂态分析正确性

## 使用案例

### DC 分析案例
  分析如图所示的直流电路。  
  ![DC 分析案例](https://lpsa.swarthmore.edu/Systems/Electrical/mna/images/MNA2.ex1.gif)
  
```
public static Circuit DCTestCircuit() {
 var testCkt = new Circuit("DCTestCircuit");
 var gnd = testCkt.Ground;
 var node1 = testCkt.GenNode();
 var node2 = testCkt.GenNode();
 var node3 = testCkt.GenNode();
 testCkt.AddComponent(new Resistor("R1", 2, node1, gnd));
 testCkt.AddComponent(new Resistor("R2", 4, node2, node3));
 testCkt.AddComponent(new Resistor("R3", 8, node2, gnd));
 testCkt.AddComponent(new VSource("Vs1", 32, node2, node1));
 testCkt.AddComponent(new VSource("Vs2", 20, node3, gnd));
 return testCkt;
}

[Fact]
public void DCTestCircuit_SolveX() {
 var builder = Vector < double > .Build;
 var testCkt = DCTestCircuit();
 double[] shouldArr = {
  -8,
  24,
  20,
  -4,
  1
 };
 var should = builder.DenseOfArray(shouldArr);

 Assert.True(Precision.AlmostEqual(DCAnalysis.SolveX(testCkt), should, 8));
}
```
  DCAnalysis.SolveX() 方法输入为一 Circuit 对象，输出为 Vector < double > 变量，
  其中 Vector 是 MathNet.Numerics.LinearAlgebra 中定义的向量类。  
  该向量的构成为：前若干项为各节点电压（地线电压为0，不出现在本向量中），后若干项为通过电路中各电压源的电流的相反数。

### 暂态分析案例
  分析 RL 电路阶跃响应。
  
```
public static Circuit TransientTestCkt() {
 var testCkt = new Circuit("testTransAnalysis");
 var gnd = testCkt.Ground;
 var node0 = testCkt.GenNode();
 var node1 = testCkt.GenNode();
 testCkt.AddComponent(new Resistor("R1", 1E3, node0, node1));
 testCkt.AddComponent(new Inductor("L1", 1, node1, gnd));

 var vs = new VSource("vStep", 1, node0, gnd);
 vs.SetStep(0, 0, 1E-9);
 testCkt.AddComponent(vs);
 return testCkt;
}

static void TestTransAnalysis() {
 var ckt = TransientTestCkt();
 var data = new TransientAnalysisData(ckt, 1E-5, 6E-3);
 TransientAnalysis.Analyze(ckt, ref data);
 var dataVList = new List < double > ();
 foreach(var item in data.Result) {
  dataVList.Add(item[0] - item[1]);
 }
 double[] dataV = dataVList.ToArray();
 
 int pointCount = dataV.Length;
 double[] dataXs = ScottPlot.DataGen.Consecutive(pointCount);

 var plt = new ScottPlot.Plot();
 plt.PlotScatter(dataXs, dataV);
 plt.Title("TestTransAnalysis");
 plt.XLabel("Time/(10us)");
 plt.YLabel("Potential/V");
 plt.SaveFig("./Plots/TestTransAnalysis.png");
}
```
  我们首先初始化 TransientAnalysisData 对象，指定模拟步长和总时长。上例中步长 1E-5，总时长 6E-3 。  
  TransientAnalysis.Analyze() 方法输入为一 Circuit 对象和一 TransientAnalysisData 的引用，并将结果以 List < Vector < double >> 形式输出到 TransientAnalysisData.Result 属性中，
  其中 Vector 是 MathNet.Numerics.LinearAlgebra 中定义的向量类。  
  List 中各向量的构成为：前若干项为各节点电压（地线电压为0，不出现在本向量中），后若干项为通过电路中各电压源的电流的相反数。  
  这里我们使用 ScottPlot 绘图库验证结果准确性。

## 参考资料
- [zupolgec/circuit-simulator](https://github.com/zupolgec/circuit-simulator)
- [circuit-sandbox](https://github.com/willymcallister/circuit-sandbox)
- [Analysis of Circuits](https://lpsa.swarthmore.edu/Systems/Electrical/mna/MNA1.html)
- [Qucs - Technical Papers](http://qucs.sourceforge.net/tech/)
