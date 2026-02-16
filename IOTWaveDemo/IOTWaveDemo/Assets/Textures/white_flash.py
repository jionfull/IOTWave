from PIL import Image, ImageDraw

"""
白闪纹理生成脚本
独立运行生成白闪纹理
"""

def create_white_flash_texture():
    """创建白闪纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    draw.rectangle([0, 0, 8, 16], fill="#FFFFFF")

    return img

def main():
    """主函数"""
    print("生成白闪纹理...")
    img = create_white_flash_texture()
    img.save("white_flash.png")
    print("白闪纹理已生成: white_flash.png")

if __name__ == "__main__":
    main()