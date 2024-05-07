function [Phi,PhiPlus,PhiMinus,Pi,W,P,d]=PROMETHEE(NoOfCriteria,NoOfAlternatives,E,W,PrefDirection,PrefFun,q,p,s)
%PROMETHEE function
	%if preference direction is min then multiply N by -1
	for i=1:NoOfAlternatives
		for j=1:NoOfCriteria
			if PrefDirection(j)==2
				E(i,j)=E(i,j)*-1;
			end
		end
	end
	%computation of fuzzy devation and mapping to unicriterion preference degrees
	[P,d]=mapDeviation(NoOfAlternatives,NoOfCriteria,E,PrefFun,q,p,s);
	%defuzzification of weights and normalization to 1 
	W=normalizeWeight(NoOfCriteria,W);
	%preference aggregation
	[Phi,PhiPlus,PhiMinus,Pi]=aggrPreference(NoOfAlternatives,NoOfCriteria,P,W);
end
