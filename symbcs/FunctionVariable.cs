﻿using System;

public class FunctionVariable : Variable
{
	public string fname;
	public Algebraic arg;
	public LambdaAlgebraic la;
	public FunctionVariable(string fname, Algebraic arg, LambdaAlgebraic la)
	{
		this.fname = fname;
		this.arg = arg;
		this.la = la;
	}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Algebraic deriv(Variable x) throws JasymcaException
	public override Algebraic deriv(Variable x)
	{
		if (Equals(x))
		{
			return Zahl.ONE;
		}
		if (!arg.depends(x))
		{
			return Zahl.ZERO;
		}
		if (la == null)
		{
			throw new JasymcaException("Can not differentiate " + fname + "  : No definition.");
		}
		string diffrule = la.diffrule;
		if (diffrule == null)
		{
			throw new JasymcaException("Can not differentiate " + fname + " : No rule available.");
		}
		Algebraic y = Lambda.evalx(diffrule, arg);
		return y.mult(arg.deriv(x));
	}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Algebraic integrate(Variable x) throws JasymcaException
	public virtual Algebraic integrate(Variable x)
	{
		arg = arg.reduce();
		if (la == null)
		{
			throw new JasymcaException("Can not integrate " + fname);
		}
		return la.integrate(arg, x);
	}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Algebraic create(String f, Algebraic arg) throws JasymcaException
	public static Algebraic create(string f, Algebraic arg)
	{
		arg = arg.reduce();
		object fl = Lambda.pc.env.getValue(f);
		if (fl != null && fl is LambdaAlgebraic)
		{
			Algebraic r = ((LambdaAlgebraic)fl).f_exakt(arg);
			if (r != null)
			{
				return r;
			}
			if (arg is Unexakt)
			{
				return ((LambdaAlgebraic)fl).f((Zahl)arg);
			}
		}
		else
		{
			fl = null;
		}
		return new Polynomial(new FunctionVariable(f,arg,(LambdaAlgebraic)fl));
	}
	public override bool Equals(object x)
	{
		return x is FunctionVariable && fname.Equals(((FunctionVariable)x).fname) && arg.Equals(((FunctionVariable)x).arg);
	}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Algebraic value(Variable var, Algebraic x) throws JasymcaException
	public override Algebraic value(Variable @var, Algebraic x)
	{
		if (Equals(@var))
		{
			return x;
		}
		else
		{
			x = arg.value(@var, x);
			Algebraic r = la.f_exakt(x);
			if (r != null)
			{
				return r;
			}
			if (x is Unexakt)
			{
				return la.f((Zahl)x);
			}
			return new Polynomial(new FunctionVariable(fname,x,la));
		}
	}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean smaller(Variable v) throws JasymcaException
	public override bool smaller(Variable v)
	{
		if (v == SimpleVariable.top)
		{
			return true;
		}
		if (v is SimpleVariable)
		{
			return false;
		}
		if (!((FunctionVariable)v).fname.Equals(fname))
		{
			return fname.CompareTo(((FunctionVariable)v).fname) < 0;
		}
		if (arg.Equals(((FunctionVariable)v).arg))
		{
			return false;
		}
		if (arg is Polynomial && ((FunctionVariable)v).arg is Polynomial)
		{
			Polynomial a = (Polynomial)arg;
			Polynomial b = (Polynomial)((FunctionVariable)v).arg;
			if (!a.@var.Equals(b.@var))
			{
				return a.@var.smaller(b.@var);
			}
			if (a.degree() != b.degree())
			{
				return a.degree() < b.degree();
			}
			for (int i = a.a.Length - 1; i >= 0; i--)
			{
				if (!a.a[i].Equals(b.a[i]))
				{
					if (a.a[i] is Zahl && b.a[i] is Zahl)
					{
						return ((Zahl)a.a[i]).smaller((Zahl)b.a[i]);
					}
					return a.a[i].norm() < b.a[i].norm();
				}
			}
		}
		return false;
	}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Variable cc() throws JasymcaException
	public override Variable cc()
	{
		if (fname.Equals("exp") || fname.Equals("log") || fname.Equals("sqrt"))
		{
			return new FunctionVariable(fname, arg.cc(),la);
		}
		throw new JasymcaException("Can't calculate cc for Function " + fname);
	}
	public override string ToString()
	{
		string a = arg.ToString();
		if (a.StartsWith("(", StringComparison.Ordinal) && a.EndsWith(")", StringComparison.Ordinal))
		{
			return fname + a;
		}
		else
		{
			return fname + "(" + a + ")";
		}
	}
}