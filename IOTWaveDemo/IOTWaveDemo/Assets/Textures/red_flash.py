from PIL import Image, ImageDraw

"""
红闪纹理生成脚本
独立运行生成红闪纹理
"""

def create_red_flash_texture():
    """创建红闪纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    draw.rectangle([0, 0, 8, 16], fill="#FF0000")
    
    return img

def main():
    """主函数"""
    print("生成红闪纹理...")
    img = create_red_flash_texture()
    img.save("red_flash.png")
    print("红闪纹理已生成: red_flash.png")

if __name__ == "__main__":
    main()