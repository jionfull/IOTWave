from PIL import Image, ImageDraw

"""
蓝灯纹理生成脚本
独立运行生成蓝灯纹理
"""

def create_blue_light_texture():
    """创建蓝灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 蓝色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#0000FF")
    
    return img

def main():
    """主函数"""
    print("生成蓝灯纹理...")
    img = create_blue_light_texture()
    img.save("blue_light.png")
    print("蓝灯纹理已生成: blue_light.png")

if __name__ == "__main__":
    main()