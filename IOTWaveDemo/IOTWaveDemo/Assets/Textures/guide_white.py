from PIL import Image, ImageDraw

"""
引导白灯纹理生成脚本
独立运行生成引导白灯纹理
"""

def create_guide_white_texture():
    """创建引导白灯纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 白色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FFFFFF")
    
    return img

def main():
    """主函数"""
    print("生成引导白灯纹理...")
    img = create_guide_white_texture()
    img.save("guide_white.png")
    print("引导白灯纹理已生成: guide_white.png")

if __name__ == "__main__":
    main()