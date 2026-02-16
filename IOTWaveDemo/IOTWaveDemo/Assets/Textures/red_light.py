from PIL import Image, ImageDraw

"""
红灯纹理生成脚本
独立运行生成红灯纹理
"""

def create_red_light_texture():
    """创建红灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 红色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FF0000")
    
    return img

def main():
    """主函数"""
    print("生成红灯纹理...")
    img = create_red_light_texture()
    img.save("red_light.png")
    print("红灯纹理已生成: red_light.png")

if __name__ == "__main__":
    main()