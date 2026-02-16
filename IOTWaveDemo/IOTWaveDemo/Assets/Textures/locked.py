from PIL import Image, ImageDraw

"""
锁闭纹理生成脚本
独立运行生成锁闭纹理
"""

def create_locked_texture():
    """创建锁闭纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 白色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FFFFFF")
    
    return img

def main():
    """主函数"""
    print("生成锁闭纹理...")
    img = create_locked_texture()
    img.save("locked.png")
    print("锁闭纹理已生成: locked.png")

if __name__ == "__main__":
    main()