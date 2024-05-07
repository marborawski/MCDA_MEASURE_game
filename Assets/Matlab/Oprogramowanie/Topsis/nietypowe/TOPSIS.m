clear all

nazwa_pliku = 'dane_do_obliczen_2016';
plik_wynikowy = 'TOPSIS_wyniki_2016';

%zmienna = 3;panstwo = 10;
zmienna = 6;panstwo = 9;
multiStd = 6;
stepStd = 0.25;

liczbaKlas = 4;

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
% lata{t} = '2014';t = t + 1;
%  lata{t} = '2015';t = t + 1;
lata{t} = '2016';t = t + 1;


% lambda=1;
% s_lambda = -0.15*lambda+0.3;

[a b param] = xlsread(nazwa_pliku,'parametry');

for ii = 2:size(param,1)
    zmienne{ii-1} = param{ii,1};
    wagi{ii-1} = param{ii,2};
    charakter{ii-1} = param{ii,3};
end

wagi = convert(wagi);

[data nazwy_wariantow_decyzyjnych code] = readData(nazwa_pliku,lata);

%saveDataXLS('Dane',data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],[]);

[data color] = interpolacja(data);

%saveDataXLS('Interpolacja',data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

wagi2 = wagi/sum(wagi);

tmp = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
%    tmp(ii) = sum(res);
    tmp(ii) = sqrt(sum(res.*res));
end

tmp_dane_entropy_norm = cell(1,length(data));
for k = 1:length(data)
    tmp_dane_entropy_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        tmp_dane_entropy_norm{k}(:,ii) = data{k}(:,ii)/tmp(ii);
    end
end

weight_entropy = zeros(1,size(tmp_dane_entropy_norm{1},2));
for ii = 1:size(tmp_dane_entropy_norm{1},2)
    res = [];
    for k = 1:length(tmp_dane_entropy_norm)
        res = [res; tmp_dane_entropy_norm{k}(:,ii)];
    end
    weight_entropy(ii) = -(sum(res.*log(res)))/(log(length(res)));
end

weight_entropy = 1 - weight_entropy;

p = find(~isnan(weight_entropy));

weight_entropy = weight_entropy/sum(weight_entropy(p));


weight3 = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    weight3(ii) = std(res)/mean(res);
end

weight3 = weight3/sum(weight3);

%wagi2 = weight3;

tmp = data{1}(:,zmienna);

mean_ = mean(tmp);
std_ = std(tmp);

nn = 0;
for iii = mean_ - multiStd*std_:stepStd*std_:mean_ + multiStd*std_
    nn = nn + 1;
end



tmp = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
%    tmp(ii) = sum(res);
    tmp(ii) = sqrt(sum(res.*res));
end

dane_norm = cell(1,length(data));
for k = 1:length(data)
    dane_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        dane_norm{k}(:,ii) = data{k}(:,ii)/tmp(ii);
    end
end

%saveDataXLS('Normowanie',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

dane_norm_wagi = cell(1,length(dane_norm));
for k = 1:length(dane_norm)
    dane_norm_wagi{k} = zeros(size(dane_norm{k}));
    for ii = 1:size(dane_norm{1},2)
        dane_norm_wagi{k}(:,ii) = dane_norm{k}(:,ii)*wagi2(ii);
    end
end

%saveDataXLS('NormowanieWagi',dane_norm_wagi,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


idealne = zeros(1,size(dane_norm_wagi{1},2));
antyidelane = zeros(1,size(dane_norm_wagi{1},2));
for ii = 1:size(dane_norm_wagi{1},2)
    res = [];
    for k = 1:length(dane_norm_wagi)
        res = [res;dane_norm_wagi{k}(:,ii)];
    end
    if strcmp(charakter(ii),'s') == 1
        idealne(ii) = max(res);
        antyidelane(ii) = min(res);
    else
        idealne(ii) = min(res);
        antyidelane(ii) = max(res);
    end
end

d_plus = zeros(size(dane_norm_wagi{1},1),1);
d_minus = zeros(size(dane_norm_wagi{1},1),1);

S_ = cell(1,length(dane_norm_wagi));
for k = 1:length(dane_norm_wagi)
    S_{k} = zeros(size(dane_norm_wagi{1},1),2);
    for ii = 1:size(dane_norm_wagi{1},1)
        d_plus(ii) = sqrt(sum(abs((idealne-dane_norm_wagi{k}(ii,:)).^2)));
        d_minus(ii) = sqrt(sum(abs((antyidelane-dane_norm_wagi{k}(ii,:)).^2)));
        S_{k}(ii,1) = d_minus(ii)/(d_plus(ii)+ d_minus(ii));
    end

    S_{k}(:,2) = klasyfikacja(S_{k}(:,1),liczbaKlas);
end

S2_{1} = [S_{1}(1:panstwo-1,:);S_{1}(panstwo+1:end,:)];

[a,b] = sort(S2_{1}(:,1),'descend');

for ii = 1:panstwo-1
    code2{ii} = code{ii};
    nazwy_wariantow_decyzyjnych2{ii} = nazwy_wariantow_decyzyjnych{ii};
end
for ii = panstwo + 1:length(nazwy_wariantow_decyzyjnych)
    nazwy_wariantow_decyzyjnych2{ii - 1} = nazwy_wariantow_decyzyjnych{ii};
    code2{ii-1} = code{ii};
end

for ii = 1:length(nazwy_wariantow_decyzyjnych2)
    nazwy_wariantow_decyzyjnych3{ii} = nazwy_wariantow_decyzyjnych2{b(ii)};
    code3{ii} = code2{b(ii)};
end

S2_{1}(:,1) = S2_{1}(b,1);
%S2_{1}(:,2) = 0;
[a,b_] = sort(S2_{1}(:,1),'descend');



start = 0;
iiii = 0;

tmp_var = data{1}(panstwo,zmienna);

for iii = tmp_var - multiStd*std_:stepStd*std_:tmp_var + multiStd*std_
%for iii = mean_ - multiStd*std_:stepStd*std_:mean_ + multiStd*std_
    data{1}(panstwo,zmienna) = iii;
           
    tmp = zeros(1,size(data{1},2));
    for ii = 1:size(data{1},2)
        res = [];
        for k = 1:length(data)
            res = [res; data{k}(:,ii)];
        end
%        tmp(ii) = sum(res);
        tmp(ii) = sqrt(sum(res.*res));
    end

    dane_norm = cell(1,length(data));
    for k = 1:length(data)
        dane_norm{k} = zeros(size(data{k}));
        for ii = 1:size(data{1},2)
            dane_norm{k}(:,ii) = data{k}(:,ii)/tmp(ii);
        end
    end

    %saveDataXLS('Normowanie',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

    dane_norm_wagi = cell(1,length(dane_norm));
    for k = 1:length(dane_norm)
        dane_norm_wagi{k} = zeros(size(dane_norm{k}));
        for ii = 1:size(dane_norm{1},2)
            dane_norm_wagi{k}(:,ii) = dane_norm{k}(:,ii)*wagi2(ii);
        end
    end

    %saveDataXLS('NormowanieWagi',dane_norm_wagi,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


    idealne = zeros(1,size(dane_norm_wagi{1},2));
    antyidelane = zeros(1,size(dane_norm_wagi{1},2));
    for ii = 1:size(dane_norm_wagi{1},2)
        res = [];
        for k = 1:length(dane_norm_wagi)
            res = [res;dane_norm_wagi{k}(:,ii)];
        end
        if strcmp(charakter(ii),'s') == 1
            idealne(ii) = max(res);
            antyidelane(ii) = min(res);
        else
            idealne(ii) = min(res);
            antyidelane(ii) = max(res);
        end
    end

    d_plus = zeros(size(dane_norm_wagi{1},1),1);
    d_minus = zeros(size(dane_norm_wagi{1},1),1);

    if start == 0
        S = cell(1,length(dane_norm_wagi));
    end
    for k = 1:length(dane_norm_wagi)
        if start == 0
            S{k} = zeros(size(dane_norm_wagi{1},1),2*nn);
        end
        for ii = 1:size(dane_norm_wagi{1},1)
            d_plus(ii) = sqrt(sum(abs((idealne-dane_norm_wagi{k}(ii,:)).^2)));
            d_minus(ii) = sqrt(sum(abs((antyidelane-dane_norm_wagi{k}(ii,:)).^2)));
            S{k}(ii,1 + iiii*2) = d_minus(ii)/(d_plus(ii)+ d_minus(ii));
        end
        
        S{k}(:,2 + iiii*2) = klasyfikacja(S{k}(:,1 + iiii*2),liczbaKlas);
    end


    
    start = 1;
    iiii = iiii + 1;
end

tmp = cell(1,1);
tmp{1} = 'Wartosc miary';
tmp{2} = 'Klasa';


saveDataXLS(plik_wynikowy,S,lata,tmp,nazwy_wariantow_decyzyjnych,code,[],[]);

S2{1} = [S{1}(1:panstwo-1,:);S{1}(panstwo+1:end,:)];

% S2{1}(:,1) = S2{1}(b,1);
% S2{1}(:,2) = 0;
% [a,b_] = sort(S2{1}(:,1),'descend');

for ii = 1:2:size(S2{1},2)
    S2{1}(:,ii) = S2{1}(b,ii);
    [a,b2] = sort(S2{1}(:,ii),'descend');
    S2{1}(:,ii + 1) = abs(b2 - b_);
end

saveDataXLS('wynik',S2,lata,tmp,nazwy_wariantow_decyzyjnych3,code3,[],[]);


clear tmp;
s = mean_ - multiStd*std_;
s2 = -multiStd;

jj = 1;
x1 = [];
x2 = [];
y1 = [];
y2 = [];
for ii = 1:2:size(S2{1},2)
    tmp{1,jj} = s; x1 = [x1 s];
    tmp{2,jj} = s2; x2 = [x2 s2]; 
    tmp{3,jj} = sum(S2{1}(:,ii + 1));y1 = [y1 tmp{3,jj}];
    tmp{4,jj} = sum(abs(S2{1}(:,ii) - S2_{1}(:,1))); y2 = [y2 tmp{4,jj}];
    s = s + stepStd*std_;
    s2 = s2 + stepStd;
    jj = jj + 1;
end

xlswrite('wynik2.xls',tmp);

figure(2);
plot(x2,y1);

