from PIL import Image, ImageDraw

"""
绿黄灯灭灯纹理生成脚本
独立运行生成绿黄灯灭灯纹理
"""

def create_green_yellow_extinguished_texture():
    """创建绿黄灯灭灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)

    # 上半部分绿色填充
    draw.rectangle([0, 0, 15, 7], fill="#00FF00")

    # 下半部分黄色填充
    draw.rectangle([0, 8, 15, 15], fill="#FFFF00")

    # 绘制黑色X标记
    draw.line([(0, 0), (15, 15)], fill="#000000", width=1)
    draw.line([(0, 15), (15, 0)], fill="#000000", width=1)

    return img

def main():
    """主函数"""
    print("生成绿黄灯灭灯纹理...")
    img = create_green_yellow_extinguished_texture()
    img.save("green_yellow_extinguished.png")
    print("绿黄灯灭灯纹理已生成: green_yellow_extinguished.png")

if __name__ == "__main__":
    main()
