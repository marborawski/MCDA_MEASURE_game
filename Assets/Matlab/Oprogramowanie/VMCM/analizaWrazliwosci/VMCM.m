clear all

%nazwa_pliku = 'dane-2005-2009';
nazwa_pliku = 'dane_do_obliczen_2016';
nazwa_pliku_std = 'dane_do_obliczen_2016_std';
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
[data_std] = readData(nazwa_pliku_std,lata);

mean_ = data_{1};
std_ = data_std{1};




result = zeros(size(data_{1},1),N);
resultClass = zeros(size(data_{1},1),N);

for iii = 1:N
    
    data = zeros(size(data_{1}));

    for k = 1:size(data,1)
        for ii = 1:size(data,2)
            data(k,ii) = mean_(k,ii) + std_(k,ii)*randn(1);
        end
    end
    
    tmp0 = zeros(1,size(data,2));
    tmp1 = zeros(1,size(data,2));
    for ii = 1:size(data,2)
        res = [];
        res = data(:,ii);
        tmp0(ii) = mean(res);
        tmp2(ii) = std(res);
        s = 0;
        n = 0;
        for k = 1:length(res)
            if abs(res(k)) <= tmp0(ii) + p* tmp2(ii)
                s = s + (res(k) - tmp0(ii)).^2;
                n = n + 1;
            end
        end
%        tmp1(ii) = sqrt(s/n);
        tmp1(ii) = sqrt(sum(res.*res));
    end
    

    dane_norm = zeros(size(data));
    for ii = 1:size(data,2)
        dane_norm(:,ii) = (data(:,ii) - tmp0(ii))/tmp1(ii);
    end

    dane_norm_wagi = zeros(size(dane_norm));
    for ii = 1:size(dane_norm,2)
        dane_norm_wagi(:,ii) = dane_norm(:,ii)*wagi2(ii);
    end

    idealne = zeros(1,size(dane_norm_wagi,2));
    antyidelane = zeros(1,size(dane_norm_wagi,2));
    for ii = 1:size(dane_norm_wagi,2)
        res = dane_norm_wagi(:,ii);
        tmp = quantile(res,[min_ max_]);
        if strcmp(charakter(ii),'s') == 1
            idealne(ii) = tmp(2);
            antyidelane(ii) = tmp(1);
        else
            idealne(ii) = tmp(1);
            antyidelane(ii) = tmp(2);
        end
    end

    idealne2 = idealne - antyidelane; 

    for ii = 1:size(dane_norm_wagi,1)
        result(ii,iii) = sum((dane_norm_wagi(ii,:) - antyidelane).*idealne2)/sum(idealne2.^2);
    end
    resultClass(:,iii) = klasyfikacja(result(:,iii),liczbaKlas);
    
end

Rs = zeros(1,N-1);
rank = resultClass(:,1);
for ii = 2:N
    Rs(ii) = sum(abs(rank - resultClass(:,ii)));
end

histRs = zeros(1,max(Rs)+ 1);

for ii = 1:N-1
    histRs(Rs(ii) + 1) = histRs(Rs(ii) + 1) + 1;
end

tmp = cell(1,1);
tmp{1,1} = 'wartoœæ';
tmp{1,2} = 'histogram Rs';

for ii = 1:length(histRs)
    tmp{ii + 1,1} = ii-1;
    tmp{ii + 1,2} = histRs(ii); 
end

delete('wynik_histogram_Rs.xls');
xlswrite('wynik_histogram_Rs.xls',tmp);

figure(1);
bar([0:length(histRs)-1],histRs);
