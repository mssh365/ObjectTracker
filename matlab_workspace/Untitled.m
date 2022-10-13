close all;
video_file='鸟.mp4';
video=VideoReader(video_file);
frame_number=video.NumFrames;
savepath='D:\workspace\matlab_workspace\视频\';
cd(savepath);%进入savepath
delete * ;%清空存储文件夹
cd('D:\workspace\matlab_workspace\');%返回

%分离图片
for i=1:frame_number
    I=read(video,i);                               %读出图片
    imwrite(I,[savepath,num2str(i),'.jpg']);       %写图片
    I=[];
end

%三帧差分+边缘检测
for i=1:frame_number-2
    I1=rgb2gray(imread([savepath,num2str(i),'.jpg'])); %依次读取每一幅图像
    I2=rgb2gray(imread([savepath,num2str(i+1),'.jpg']));
    I3=rgb2gray(imread([savepath,num2str(i+2),'.jpg']));
    alter_1=I2-I1-0;%二帧差分
    alter_2=I3-I2-0;%二帧差分
    alter_3=immultiply(alter_1,alter_2);%取交集,三帧差分
    se=strel('disk',3);
    for j=1:3
        alter_3=imdilate(alter_3,se);
    end
    %对第二帧进行canny边缘检测
    edge1=edge(I2,'canny');
    B=[0 1 0
        1 1 1
        0 1 0];
    edge2=imdilate(edge1,B);
    edge2=imopen(edge2,B);
    edge2=immultiply(edge2,alter_3);%取交集
    edge2=im2bw(edge2, 0.1);
    imwrite(edge2,[savepath,'!0_',num2str(i),'.jpg']);       %写图片
end

%对结果形态学处理
for i=1:frame_number-2
    I1=imread([savepath,'!0_',num2str(i),'.jpg']); %依次读取每一幅图像
    se=strel('disk',10);
    alter1=imclose(I1,se);
    alter1=im2bw(alter1,0.5);
    imwrite(alter1,[savepath,'!1_',num2str(i),'.jpg']);       %写图片
end

%跟踪物体
x=-1;
y=-1;
for i=1:frame_number-2
    I1=imread([savepath,'!1_',num2str(i),'.jpg']); %依次读取每一幅图像
    %种子点的交互式选择
    if(x==-1&&y==-1)
        figure,imshow(imread([savepath,'!1_1.jpg']),[]);
        hold on;
        [y,x] = getpts;%鼠标取点  回车确定
        x = round(x(1));%选择种子点
        y = round(y(1));
    end
    %对各连通域进行标记
    imLabel = bwlabel(I1);
    label=imLabel(x,y);
    img = ismember(imLabel,label);
    %%求质心
    sumx = 0;
    sumy = 0;
    area = 0;
    [height,width] = size(img);
    for k = 1 : height
        for j = 1 : width
            if img(k,j) == 1
                sumx = sumx + k;
                sumy = sumy + j;
                area = area + 1;
            end
        end
    end
    %质心坐标
    x= fix(sumx / area);
    y= fix(sumy / area);
    x_data(i)=x;
    y_data(i)=y;
    if(i<frame_number-2)
        I2=im2gray(imread([savepath,'!1_',num2str(i+1),'.jpg'])); %依次读取下一帧
        %不断在这一帧和下一帧绘制更大的矩形直到与当前所选的区域联通
        %这一帧的矩形用于检测联通
        %防止下一帧的焦点掉出物体
        for long=0:50%矩形最大100x100
            %不断绘制4条边来扩大正方形面积
            I1(x-long:x+long,y+long)=255;
            I1(x-long:x+long,y-long)=255;
            I1(x+long,y-long:y+long)=255;
            I1(x-long,y-long:y+long)=255;
            I2(x-long:x+long,y+long)=255;
            I2(x-long:x+long,y-long)=255;
            I2(x+long,y-long:y+long)=255;
            I2(x-long,y-long:y+long)=255;
            %对各连通域进行标记
            imLabel = bwlabel(I1);
            label=imLabel(x,y);
            %%求质心
            sumx = 0;
            sumy = 0;
            area = 0;
            [height,width] = size(img);
            for k = 1 : height
                for j = 1 : width
                    if img(k,j) == 1
                        sumx = sumx + k;
                        sumy = sumy + j;
                        area = area + 1;
                    end
                end
            end
            %质心坐标
            x_2= fix(sumx / area);
            y_2= fix(sumy / area);
            imLabel = bwlabel(I1);
            label_1=imLabel(x,y);
            label_2=imLabel(x_2,y_2);
            if(label_1==label_2)
                break
            end
        end
        imwrite(I2,[savepath,'!1_',num2str(i+1),'.jpg']);       %写图片
    end
    %写图片
    imwrite(img,[savepath,'!2_',num2str(i),'.jpg']);
end

%图片转为视频
myObj = VideoWriter([savepath,'new']);%初始化avi文件
myObj.FrameRate = 24;%帧率24
open(myObj);
for i=1:frame_number-2%图像序列个数
   fname=strcat([savepath,'!1_',num2str(i),'.jpg']);
    I = imread(fname);
    %框出质心
    I(x_data(i)-10:x_data(i)+10,y_data(i)-10)=255-I(x_data(i)-10:x_data(i)+10,y_data(i)-10);
    I(x_data(i)-10:x_data(i)+10,y_data(i)+10)=255-I(x_data(i)-10:x_data(i)+10,y_data(i)+10);
    I(x_data(i)-10,y_data(i)-10:y_data(i)+10)=255-I(x_data(i)-10,y_data(i)-10:y_data(i)+10);
    I(x_data(i)+10,y_data(i)-10:y_data(i)+10)=255-I(x_data(i)+10,y_data(i)-10:y_data(i)+10);
    I(x_data(i)-11:x_data(i)+11,y_data(i)-11)=255-I(x_data(i)-11:x_data(i)+11,y_data(i)-11);
    I(x_data(i)-11:x_data(i)+11,y_data(i)+11)=255-I(x_data(i)-11:x_data(i)+11,y_data(i)+11);
    I(x_data(i)-11,y_data(i)-11:y_data(i)+11)=255-I(x_data(i)-11,y_data(i)-11:y_data(i)+11);
    I(x_data(i)+11,y_data(i)-11:y_data(i)+11)=255-I(x_data(i)+11,y_data(i)-11:y_data(i)+11);
   writeVideo(myObj,I);
end
close(myObj);

%图片直接播放
for i = 1 : frame_number-2 %遍历每一帧
    fname=strcat([savepath,'!1_',num2str(i),'.jpg']);
    I = imread(fname);
    %框出质心
    I(x_data(i)-10:x_data(i)+10,y_data(i)-10)=255-I(x_data(i)-10:x_data(i)+10,y_data(i)-10);
    I(x_data(i)-10:x_data(i)+10,y_data(i)+10)=255-I(x_data(i)-10:x_data(i)+10,y_data(i)+10);
    I(x_data(i)-10,y_data(i)-10:y_data(i)+10)=255-I(x_data(i)-10,y_data(i)-10:y_data(i)+10);
    I(x_data(i)+10,y_data(i)-10:y_data(i)+10)=255-I(x_data(i)+10,y_data(i)-10:y_data(i)+10);
    I(x_data(i)-11:x_data(i)+11,y_data(i)-11)=255-I(x_data(i)-11:x_data(i)+11,y_data(i)-11);
    I(x_data(i)-11:x_data(i)+11,y_data(i)+11)=255-I(x_data(i)-11:x_data(i)+11,y_data(i)+11);
    I(x_data(i)-11,y_data(i)-11:y_data(i)+11)=255-I(x_data(i)-11,y_data(i)-11:y_data(i)+11);
    I(x_data(i)+11,y_data(i)-11:y_data(i)+11)=255-I(x_data(i)+11,y_data(i)-11:y_data(i)+11);
    subplot (111);imshow(I);title([num2str(x_data(i)),',',num2str(y_data(i))]); %显示当前帧以及质心
    pause(0.05); %暂停系统，使人眼连贯观察到每一帧，此设为0.05秒
end
