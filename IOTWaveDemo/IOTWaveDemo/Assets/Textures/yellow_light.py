from PIL import Image, ImageDraw

"""
黄灯纹理生成脚本
独立运行生成黄灯纹理
"""

def create_yellow_light_texture():
    """创建黄灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 黄色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FFFF00")
    
    return img

def main():
    """主函数"""
    print("生成黄灯纹理...")
    img = create_yellow_light_texture()
    img.save("yellow_light.png")
    print("黄灯纹理已生成: yellow_light.png")

if __name__ == "__main__":
    main()