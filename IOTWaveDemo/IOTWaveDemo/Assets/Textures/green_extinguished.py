from PIL import Image, ImageDraw

"""
室外灭灯纹理生成脚本
独立运行生成室外灭灯纹理
"""

def create_extinguished_texture():
    """创建室外灭灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#00FF00")
    draw = ImageDraw.Draw(img)
    
    # 绘制黑色X标记
    draw.line([(0, 0), (15, 15)], fill="#000000", width=1)
    draw.line([(0, 15), (15, 0)], fill="#000000", width=1)
    
    return img

def main():
    """主函数"""
    print("生成室外灭灯纹理...")
    img = create_extinguished_texture()
    img.save("green_extinguished.png")
    print("室外灭灯纹理已生成: extinguished.png")

if __name__ == "__main__":
    main()