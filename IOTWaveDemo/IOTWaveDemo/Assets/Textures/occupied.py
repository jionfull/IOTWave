from PIL import Image, ImageDraw

"""
占用纹理生成脚本
独立运行生成占用纹理
"""

def create_occupied_texture():
    """创建占用纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 红色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#FF0000")
    
    return img

def main():
    """主函数"""
    print("生成占用纹理...")
    img = create_occupied_texture()
    img.save("occupied.png")
    print("占用纹理已生成: occupied.png")

if __name__ == "__main__":
    main()