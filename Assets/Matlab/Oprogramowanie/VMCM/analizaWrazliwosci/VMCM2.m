clear all

%nazwa_pliku = 'dane-2005-2009';
nazwa_pliku = 'dane_do_obliczen_2016';
%nazwa_pliku_std = 'dane_do_obliczen_2016_std';
%nazwa_pliku = 'dane-2010-2016';
%plik_wynikowy = 'VIKOR_wynik-2005-2009';
%plik_wynikowy = 'VIKOR_wynik-2005-2016';
plik_wynikowy = 'VMCM_wyniki_2016';

liczbaKlas = 4;

N = 10000;

p = 1;
 
zmienna = 3;panstwo = 10;
%zmienna = 6;panstwo = 9;
multiStd = 6;
stepStd = 0.25;

min_ = 0.25;
max_ = 0.75;

t = 1;
%  lata{t} = '2005';t = t + 1;
%  lata{t} = '2006';t = t + 1;
%  lata{t} = '2007';t = t + 1;
%  lata{t} = '2008';t = t + 1;
%  lata{t} = '2009';t = t + 1;
%  lata{t} = '2010';t = t + 1;
%  lata{t} = '2011';t = t + 1;
%  lata{t} = '2012';t = t + 1;
%  lata{t} = '2013';t = t + 1;
%  lata{t} = '2014';t = t + 1;
%  lata{t} = '2015';t = t + 1;
 lata{t} = '2016';t = t + 1;

[a b param] = xlsread(nazwa_pliku,'parametry');

for ii = 2:size(param,1)
    zmienne{ii-1} = param{ii,1};
    wagi{ii-1} = param{ii,2};
    charakter{ii-1} = param{ii,3};
end

wagi = convert(wagi);

wagi2 = wagi/sum(wagi);

[data_ nazwy_wariantow_decyzyjnych code] = readData(nazwa_pliku,lata);
%[data_std] = readData(nazwa_pliku_std,lata);



mean_ = mean(data_{1});
std_ = std(data_{1});


N_ = 1;

yA = zeros(size(data_{1},1),N_);
yB = zeros(size(data_{1},1),N_);
% resultClassA = zeros(size(data_{1},1),N_);
% resultClassB = zeros(size(data_{1},1),N_);

for iii = 1:N_
    
    A = zeros(N,size(data_{1},2));
    B = zeros(N,size(data_{1},2));

    for k = 1:size(A,1)
        for ii = 1:size(A,2)
            A(k,ii) = mean_(ii) + std_(ii)*randn(1);
            B(k,ii) = mean_(ii) + std_(ii)*randn(1);
        end
    end
    
    tmp0A = zeros(1,size(A,2));
    tmp0B = zeros(1,size(A,2));
    tmp1A = zeros(1,size(A,2));
    tmp1B = zeros(1,size(A,2));
    for ii = 1:size(A,2)
        resA = A(:,ii);
        resB = B(:,ii);
        tmp0A(ii) = mean(resA);
        tmp0B(ii) = mean(resB);
        tmp2A(ii) = std(resA);
        s = 0;
        n = 0;
        for k = 1:length(resA)
            if abs(resA(k)) <= tmp0A(ii) + p* tmp2A(ii)
                s = s + (resA(k) - tmp0A(ii)).^2;
                n = n + 1;
            end
        end
%        tmp1(ii) = sqrt(s/n);
        tmp1A(ii) = sqrt(sum(resA.*resA));
        tmp1B(ii) = sqrt(sum(resB.*resB));
    end
    

    A_norm = zeros(size(A));
    B_norm = zeros(size(A));
    for ii = 1:size(A,2)
        A_norm(:,ii) = (A(:,ii) - tmp0A(ii))/tmp1A(ii);
        B_norm(:,ii) = (B(:,ii) - tmp0B(ii))/tmp1B(ii);
    end

    A_norm_wagi = zeros(size(A_norm));
    B_norm_wagi = zeros(size(B_norm));
    for ii = 1:size(A_norm,2)
        A_norm_wagi(:,ii) = A_norm(:,ii)*wagi2(ii);
        B_norm_wagi(:,ii) = B_norm(:,ii)*wagi2(ii);
    end

    A_idealne = zeros(1,size(A_norm_wagi,2));
    B_idealne = zeros(1,size(B_norm_wagi,2));
    A_antyidelane = zeros(1,size(A_norm_wagi,2));
    B_antyidelane = zeros(1,size(B_norm_wagi,2));
    for ii = 1:size(A_norm_wagi,2)
        resA = A_norm_wagi(:,ii);
        resB = B_norm_wagi(:,ii);
        tmpA = quantile(resA,[min_ max_]);
        tmpB = quantile(resB,[min_ max_]);
        if strcmp(charakter(ii),'s') == 1
            A_idealne(ii) = tmpA(2);
            B_idealne(ii) = tmpB(2);
            A_antyidelane(ii) = tmpA(1);
            B_antyidelane(ii) = tmpB(1);
        else
            A_idealne(ii) = tmpA(1);
            B_idealne(ii) = tmpB(1);
            A_antyidelane(ii) = tmpA(2);
            B_antyidelane(ii) = tmpB(2);
        end
    end

    A_idealne2 = A_idealne - A_antyidelane; 
    B_idealne2 = B_idealne - B_antyidelane; 

    for ii = 1:size(A_norm_wagi,1)
        yA(ii,iii) = sum((A_norm_wagi(ii,:) - A_antyidelane).*A_idealne2)/sum(A_idealne2.^2);
        yB(ii,iii) = sum((B_norm_wagi(ii,:) - B_antyidelane).*B_idealne2)/sum(B_idealne2.^2);
    end
%    resultClassA(:,iii) = klasyfikacja(resultA(:,iii),liczbaKlas);
%    resultClassB(:,iii) = klasyfikacja(resultB(:,iii),liczbaKlas);
    
end


f02 = (sum(yA)/N)^2;

yC = zeros(size(data_{1},1),N_);

S = zeros(1,size(data_{1},2));
ST = zeros(1,size(data_{1},2));
PE = zeros(1,size(data_{1},2));


for iii = 1:size(data_{1},2)
    C = B;
    C(:,iii) = A(:,iii);
    
    
    tmp0C = zeros(1,size(C,2));
    tmp1C = zeros(1,size(C,2));
    for ii = 1:size(C,2)
        resC = C(:,ii);
        tmp0C(ii) = mean(resC);
%        tmp1(ii) = sqrt(s/n);
        tmp1C(ii) = sqrt(sum(resC.*resC));
    end
    

    C_norm = zeros(size(C));
    for ii = 1:size(C,2)
        C_norm(:,ii) = (C(:,ii) - tmp0C(ii))/tmp1C(ii);
    end

    C_norm_wagi = zeros(size(C_norm));
    for ii = 1:size(C_norm,2)
        C_norm_wagi(:,ii) = C_norm(:,ii)*wagi2(ii);
    end

    C_idealne = zeros(1,size(C_norm_wagi,2));
    C_antyidelane = zeros(1,size(C_norm_wagi,2));
    for ii = 1:size(C_norm_wagi,2)
        resC = C_norm_wagi(:,ii);
        tmpC = quantile(resC,[min_ max_]);
        if strcmp(charakter(ii),'s') == 1
            C_idealne(ii) = tmpC(2);
            C_antyidelane(ii) = tmpC(1);
        else
            C_idealne(ii) = tmpC(1);
            C_antyidelane(ii) = tmpC(2);
        end
    end

    C_idealne2 = C_idealne - C_antyidelane; 

    for ii = 1:size(A_norm_wagi,1)
        yC(ii) = sum((C_norm_wagi(ii,:) - C_antyidelane).*C_idealne2)/sum(C_idealne2.^2);
    end

    S(iii) = (sum(yA.*yC)/N - f02)/(sum(yA.*yA)/N - f02);
    ST(iii) = 1-(sum(yB.*yC)/N - f02)/(sum(yA.*yA)/N - f02);
%     S(ii) = (sum(yA.*yC) - f02)/(sum(yA.*yA) - f02);
%     ST(ii) = 1-(sum(yB.*yC) - f02)/(sum(yA.*yA) - f02);
    PE(iii) = (0.6745/sqrt(N))*sqrt(sum((yA.*yC).^2) - sum(yA.*yC)^2);
end


tmp = cell(1,1);
tmp{1,1} = 'wskaŸnik';
tmp{1,2} = 'S';
tmp{1,3} = 'St';

for ii = 1:length(S)
    tmp{ii + 1,1} = zmienne{ii};
    tmp{ii + 1,2} = S(ii); 
    tmp{ii + 1,3} = ST(ii); 
end

delete('wynik_S_St.xls');
xlswrite('wynik_S_St.xls',tmp);


