from PIL import Image, ImageDraw

"""
黄闪纹理生成脚本
独立运行生成黄闪纹理
"""

def create_yellow_flash_texture():
    """创建黄闪纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    draw.rectangle([0, 0, 8, 16], fill="#FFFF00")

    return img

def main():
    """主函数"""
    print("生成黄闪纹理...")
    img = create_yellow_flash_texture()
    img.save("yellow_flash.png")
    print("黄闪纹理已生成: yellow_flash.png")

if __name__ == "__main__":
    main()