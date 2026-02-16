from PIL import Image, ImageDraw

"""
引导白灯灭灯纹理生成脚本
独立运行生成引导白灯灭灯纹理
"""

def create_guide_white_extinguished_texture():
    """创建引导白灯灭灯纹理"""
    # 创建16x16的白色背景图像
    img = Image.new("RGB", (16, 16), color="#FFFFFF")
    draw = ImageDraw.Draw(img)

    # 绘制黑色X标记
    draw.line([(0, 0), (15, 15)], fill="#000000", width=1)
    draw.line([(0, 15), (15, 0)], fill="#000000", width=1)

    return img

def main():
    """主函数"""
    print("生成引导白灯灭灯纹理...")
    img = create_guide_white_extinguished_texture()
    img.save("guide_white_extinguished.png")
    print("引导白灯灭灯纹理已生成: guide_white_extinguished.png")

if __name__ == "__main__":
    main()
