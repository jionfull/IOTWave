from PIL import Image, ImageDraw

"""
空闲纹理生成脚本
独立运行生成空闲纹理
"""

def create_idle_texture():
    """创建空闲纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 浅蓝色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#ADD8E6")
    
    return img

def main():
    """主函数"""
    print("生成空闲纹理...")
    img = create_idle_texture()
    img.save("idle.png")
    print("空闲纹理已生成: idle.png")

if __name__ == "__main__":
    main()