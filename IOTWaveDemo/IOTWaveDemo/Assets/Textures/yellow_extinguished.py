from PIL import Image, ImageDraw

"""
黄灯灭灯纹理生成脚本
独立运行生成黄灯灭灯纹理
"""

def create_yellow_extinguished_texture():
    """创建黄灯灭灯纹理"""
    # 创建16x16的黄色背景图像
    img = Image.new("RGB", (16, 16), color="#FFFF00")
    draw = ImageDraw.Draw(img)

    # 绘制黑色X标记
    draw.line([(0, 0), (15, 15)], fill="#000000", width=1)
    draw.line([(0, 15), (15, 0)], fill="#000000", width=1)

    return img

def main():
    """主函数"""
    print("生成黄灯灭灯纹理...")
    img = create_yellow_extinguished_texture()
    img.save("yellow_extinguished.png")
    print("黄灯灭灯纹理已生成: yellow_extinguished.png")

if __name__ == "__main__":
    main()
